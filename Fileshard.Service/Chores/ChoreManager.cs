using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fileshard.Service.Chores
{
    public class ChoreManager : INotifyPropertyChanged
    {
        private static readonly Lazy<ChoreManager> _instance = new Lazy<ChoreManager>(() => new ChoreManager());

        private float _progress;
        private string _statusText;
        private bool _isActiveChore;
        private Task _currentTask;
        private CancellationTokenSource _cancellationTokenSource;

        private ChoreManager() { }

        public static ChoreManager Instance => _instance.Value;

        public float Progress
        {
            get => _progress;
            private set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string StatusText
        {
            get => _statusText;
            private set
            {
                if (_statusText != value)
                {
                    _statusText = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsActiveChore
        {
            get => _isActiveChore;
            private set
            {
                if (_isActiveChore != value)
                {
                    _isActiveChore = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void TryDispatchChore<T>(ProgressIterator<T> iterator)
        {
            if (_currentTask != null && !_currentTask.IsCompleted)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            _currentTask = Task.Run(async () =>
            {
                IsActiveChore = true;

                void OnIteratorPropertyChanged(object sender, PropertyChangedEventArgs e)
                {
                    if (e.PropertyName == nameof(ProgressIterator<T>.Progress))
                    {
                        Progress = iterator.Progress;
                    }
                    else if (e.PropertyName == nameof(ProgressIterator<T>.StatusText))
                    {
                        StatusText = iterator.StatusText;
                    }
                    else if (e.PropertyName == nameof(ProgressIterator<T>.IsActive))
                    {
                        IsActiveChore = iterator.IsActive;
                    }
                }

                iterator.PropertyChanged += OnIteratorPropertyChanged;

                await iterator.StartIteration(token);

                iterator.PropertyChanged -= OnIteratorPropertyChanged;
            }, token);
        }

        public void CancelCurrentTask()
        {
            _cancellationTokenSource?.Cancel();
            IsActiveChore = false;
        }
    }
}
