using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using Fileshard.Frontend.Components;
using Fileshard.Service.Chores;
using Fileshard.Service.Database;
using Fileshard.Service.Repository;
using Fileshard.Service.Structs;
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
        private float _progress = 0;
        private bool _isBusy = false;

        public ObservableCollection<FileItem> Files { get; set; }

        private FileshardObject? _selectedObject;

        // Task Mutex to ensure only a single task is running at a time
        private readonly SemaphoreSlim _taskMutex = new SemaphoreSlim(1, 1);

        private void ChoreManager_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ChoreManager.Progress))
            {
                Progress = (int)ChoreManager.Instance.Progress;
            }
            else if (e.PropertyName == nameof(ChoreManager.StatusText))
            {
                StatusText = ChoreManager.Instance.StatusText;
            } else if (e.PropertyName == nameof(ChoreManager.IsActiveChore))
            {
                IsBusy = ChoreManager.Instance.IsActiveChore ? Visibility.Visible : Visibility.Hidden;
            }
        }

        public MainWindowViewModel()
        {
            _collectionRepository = new CollectionRepository();
            Files = new ObservableCollection<FileItem> { };

            ChoreManager.Instance.PropertyChanged += ChoreManager_PropertyChanged;

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
        }

        internal void DispatchMetaHasher()
        {
            var task = new FileProcessorChore(_collectionRepository, _selectedCollection.Id);
            Observable.FromAsync(() => task.GetTask())
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(async iterator =>
                {
                    ChoreManager.Instance.TryDispatchChore(iterator);
                });
        }

        public float Progress
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
