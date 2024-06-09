using System.Collections.ObjectModel;
using System.IO;
using System.Reactive.Linq;
using Fileshard.Frontend.Components;
using Fileshard.Service.Database;
using Fileshard.Service.Entities;
using Fileshard.Service.Repository;
using ReactiveUI;

namespace Fileshard.Frontend
{
    class MainWindowViewModel : ReactiveObject
    {
        private string _statusText = "";
        private string _statusCollectionText = "";
        private string _objectDetailText = "";

        private FileshardCollection? _selectedCollection;
        private readonly ICollectionRepository _collectionRepository;

        public ObservableCollection<FileItem> Files { get; set; }

        private FileshardObject? _selectedObject;

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

                            Files.Add(new FileItem { Name = item.Name, Path = item.Files.First().InternalPath, Icon = null, ObjectGuid = item.Id });
                        }
                    });
                });
        }

        public void OnFileDropped(List<String> files)
        {
            if (_selectedCollection == null)
            {
                StatusText = "Please select a collection first";
                return;
            }

            List<FileshardObject> fileshardObjects = new List<FileshardObject>();

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file).ToLower();

                if (extension != ".jpg" && extension != ".png") continue;

                StatusText = $"Loaded {Path.GetFileName(file)}";

                var obj = new FileshardObject
                {
                    Id = Guid.NewGuid(),
                    Name = Path.GetFileName(file),
                    IsImport = true,
                    ImportedAt = DateTime.Now
                };

                obj.Files.Add(new FileshardFile
                {
                    Id = Guid.NewGuid(),
                    InternalPath = file
                });

                fileshardObjects.Add(obj);

                StatusText = $"Loading {fileshardObjects.Count}/{files.Count} objects";
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
