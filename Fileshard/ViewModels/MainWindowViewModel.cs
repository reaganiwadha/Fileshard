using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Threading;
using CoenM.ImageHash;
using CoenM.ImageHash.HashAlgorithms;
using Fileshard.Frontend.Components;
using Fileshard.Frontend.Helpers;
using Fileshard.Service.Database;
using Fileshard.Service.Repository;
using Fileshard.Service.Structs;
using Microsoft.Msagl.Drawing;
using ReactiveUI;

namespace Fileshard.Frontend.ViewModels
{
    class MainWindowViewModel : ReactiveObject
    {
        private string _statusText = "";
        private string _statusCollectionText = "";
        private string _objectDetailText = "";

        private FileshardCollection? _selectedCollection;
        private readonly ICollectionRepository _collectionRepository;
        private int _progress = 60;
        private bool _isBusy = false;

        private Graph _graph;

        public ObservableCollection<FileItem> Files { get; set; }

        private FileshardObject? _selectedObject;

        // Task Mutex to ensure only a single task is running at a time
        private readonly SemaphoreSlim _taskMutex = new SemaphoreSlim(1, 1);

        public MainWindowViewModel()
        {
            _collectionRepository = new CollectionRepository();
            Files = new ObservableCollection<FileItem> { };

            Observable.FromAsync(() => _collectionRepository.GetAll())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(isEmpty =>
                {
                    if (isEmpty.Count != 0)
                    {
                        _selectedCollection = isEmpty[0];
                    } else
                    {
                        StatusText = "No collections found... Please setup - File > Create New Collection";
                    }
                });

            this.WhenAnyValue(x => x.SelectedCollection)
                  .Where(item => item != null && item.Id != Guid.Empty)
                  .Subscribe(guid =>
                  {
                      StatusCollectionText = $"Selected Collection: {guid.Title} ({guid.Id})";
                      ReloadObjects();
                  });
        }

        private void ReloadObjects()
        {
            Observable.FromAsync(() => _collectionRepository.GetObjects(_selectedCollection.Id))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(objects =>
                {
                    App.Current.Dispatcher.Invoke(() => {
                        Files.Clear();

                        foreach (var item in objects)
                        {
                            if (item.Files.Count == 0) continue;

                            Files.Add(new FileItem { Name = item.Name, Path = item.Files.First().InternalPath, ObjectGuid = item.Id });
                        }
                    });
                });
        }

        private void DispatchGraphUpdate()
        {
            Graph = new Graph();

            new Thread(() =>
            {
                var graph = new Graph();
                graph.AddNode(_selectedObject.Id.ToString());
                graph.AddNode("name");
                var name = _selectedObject.Name.WrapAt(2    0).JoinLines();

                graph.AddNode(name);

                graph.AddEdge(_selectedObject.Id.ToString(), "name");
                graph.AddEdge("name", name);

                graph.AddNode("files");
                graph.AddEdge(_selectedObject.Id.ToString(), "files");


                foreach (var file in _selectedObject.Files)
                {
                    graph.AddNode(file.Id.ToString());
                    graph.AddEdge("files", file.Id.ToString());

                    var internalPath = file.InternalPath.WrapAt(25).JoinLines();
                    var n = graph.AddNode(internalPath);

                    graph.AddEdge(file.Id.ToString(), internalPath);

                    graph.AddNode("metas");
                    graph.AddEdge(file.Id.ToString(), "metas");

                    foreach (var meta in file.Metas)
                    {
                        graph.AddNode(meta.Key);
                        graph.AddEdge("metas", meta.Key);

                        graph.AddNode(meta.Value);
                        graph.AddEdge(meta.Key, meta.Value);
                    }
                }

                Graph = graph;
            }).Start();
        }

        public void OnFileDropped(List<String> files)
        {
            if (_selectedCollection == null)
            {
                StatusText = "Please select a collection first";
                return;
            }

            List<FileshardObject> fileshardObjects = new List<FileshardObject>();

            files = _collectionRepository.FilterNonExistentFiles(files).Result;

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file).ToLower();

                if (extension != ".jpg" && extension != ".png") continue;

                StatusText = $"Loaded {Path.GetFileName(file)}";

                var obj = new FileshardObject
                {
                    Id = Guid.NewGuid(),
                    Name = Path.GetFileName(file)
                };

                obj.Files.Add(new FileshardFile
                {
                    Id = Guid.NewGuid(),
                    InternalPath = file
                });

                fileshardObjects.Add(obj);

                StatusText = $"Loading {fileshardObjects.Count}/{files.Count} objects";
            }

            if (fileshardObjects.Count == 0)
            {
                StatusText = "No valid files found in the drop";
                return;
            }

            Observable.FromAsync(() => _collectionRepository.Ingest(_selectedCollection.Id, fileshardObjects))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    StatusText = $"Ingested {files.Count} objects as imports!";
                    ReloadObjects();
                });
        }

        internal void CreateAndSelectCollection(string responseText)
        {
            Observable.FromAsync(() => _collectionRepository.Create(responseText))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(guid =>
                {
                    StatusText = $"Created new collection: {responseText}";
                    SelectedCollection = new FileshardCollection { Id = guid, Title = responseText };
                });
        }

        internal void OnObjectSelected(Guid objectGuid)
        {
            SelectedObject = _collectionRepository.GetObject(_selectedCollection.Id, objectGuid).Result;
            ObjectDetailText = $"Selected Object: {_selectedObject.Name} ({_selectedObject.Id})";
            DispatchGraphUpdate();
        }

        internal void DispatchMetaHasher()
        {
            StatusText = "Processing meta hashes...";
            IsBusy = Visibility.Visible;

            // Dispatch a sleeping thread that updates the progress bar every 100ms
            new Thread(async () =>
            {
                _taskMutex.Wait();

                var objects = await _collectionRepository.GetObjects(_selectedCollection.Id);
                // var filteredObjects = objects.Where(e => e.Files.Any(f => f.Metas.Count < 2));
                var filteredObjects = objects;

                int total = filteredObjects.Count();
                int index = 0;
                foreach (var obj in filteredObjects)
                {
                    foreach (var file in obj.Files)
                    {
                        try 
                        {
                            String hash = HashUtil.ComputeMD5(file.InternalPath);
                            await _collectionRepository.UpsertMeta("hash:md5", hash, file.Id);
                        } 
                        catch
                        {
                            continue;
                        }

                        try 
                        { 
                            var diffHash = new DifferenceHash();
                            ulong hash = 0;
                            using (var fileStream = File.OpenRead(file.InternalPath))
                            {
                                hash = diffHash.Hash(fileStream);
                            }

                            if (diffHash == null || hash == 0) continue;

                            await _collectionRepository.UpsertMeta("hash:ImageHash:diff", hash.ToString(), file.Id);
                        } catch (Exception e)
                        {
                            continue;
                        }

                        index++;
                        Progress = (int)(((float)index / total) * 100);
                        StatusText = $"Processing meta hashes... {index}/{total}";
                    }
                }

                StatusText = "Meta hashes processed!";
                IsBusy = Visibility.Hidden;
                ReloadObjects();
                _taskMutex.Release();
            }).Start();
        }

        public Graph Graph
        {
            get => _graph;
            set => this.RaiseAndSetIfChanged(ref _graph, value);
        }

        public int Progress
        {
            get => _progress;
            set => this.RaiseAndSetIfChanged(ref _progress, value);
        }

        public Visibility IsBusy
        {
            get => _isBusy ? Visibility.Visible : Visibility.Hidden;
            set => this.RaiseAndSetIfChanged(ref _isBusy, value == Visibility.Visible);
        }

        public FileshardCollection? SelectedCollection
        {
            get => _selectedCollection;
            set => this.RaiseAndSetIfChanged(ref _selectedCollection, value);
        }

        public FileshardObject? SelectedObject
        {
            get => _selectedObject;
            set => this.RaiseAndSetIfChanged(ref _selectedObject, value);
        }

        public string StatusText
        {
            get => _statusText;
            set => this.RaiseAndSetIfChanged(ref _statusText, value);
        }

        public string ObjectDetailText
        {
            get => _objectDetailText;
            set => this.RaiseAndSetIfChanged(ref _objectDetailText, value);
        }

        public string StatusCollectionText
        {
            get => _statusCollectionText;
            set => this.RaiseAndSetIfChanged(ref _statusCollectionText, value);
        }
    }
}
