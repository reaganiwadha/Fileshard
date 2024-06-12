using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fileshard.Service.Chores
{
    public abstract class ProgressIterator<T> : INotifyPropertyChanged
    {
        private float _progress;
        private string _statusText;
        private bool _isActive;
        private int _threadCount;

        protected ProgressIterator(IEnumerable<T> collection)
        {
            SyncCollection = collection ?? throw new ArgumentNullException(nameof(collection));
            Reset();
        }

        protected ProgressIterator(IAsyncEnumerable<T> collection)
        {
            AsyncCollection = collection ?? throw new ArgumentNullException(nameof(collection));
            Reset();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public IEnumerable<T> SyncCollection { get; }

        public IAsyncEnumerable<T> AsyncCollection { get; }

        public float Progress
        {
            get => _progress;
            private set
            {
                _progress = value;
                OnPropertyChanged();
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

        public bool IsActive
        {
            get => _isActive;
            private set
            {
                if (_isActive != value)
                {
                    _isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public int ThreadCount
        {
            get => _threadCount;
            set
            {
                if (_threadCount != value)
                {
                    _threadCount = value;
                    OnPropertyChanged();
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task StartIteration(CancellationToken token)
        {
            IsActive = true;
            int totalItems = AsyncCollection != null ? await CountAsync(AsyncCollection) : SyncCollection.Count();
            int processedItems = 0;

            List<Task> tasks = new List<Task>();
            object progressLock = new object(); // Lock object for progress update

            if (AsyncCollection != null)
            {
                await foreach (var item in AsyncCollection.WithCancellation(token))
                {
                    if (token.IsCancellationRequested)
                    {
                        StatusText = "Cancelled";
                        Progress = 0;
                        IsActive = false;
                        return;
                    }

                    tasks.Add(Task.Run(async () =>
                    {
                        await ProcessItemAsync(item, token);
                        Interlocked.Increment(ref processedItems);
                        float progress = (float)processedItems / totalItems * 100;

                        // Update progress and status text within lock
                        lock (progressLock)
                        {
                            Interlocked.Exchange(ref _progress, progress);
                            StatusText = BuildStatusText(processedItems, totalItems, progress);
                            Progress = progress;
                        }
                    }));

                    if (tasks.Count >= ThreadCount)
                    {
                        await Task.WhenAny(tasks);
                        tasks.RemoveAll(t => t.IsCompleted);
                    }
                }
            }
            else
            {
                foreach (var item in SyncCollection)
                {
                    if (token.IsCancellationRequested)
                    {
                        StatusText = "Cancelled";
                        Progress = 0;
                        IsActive = false;
                        return;
                    }

                    tasks.Add(Task.Run(async () =>
                    {
                        await ProcessItemAsync(item, token);
                        Interlocked.Increment(ref processedItems);
                        float progress = (float)processedItems / totalItems * 100;

                        // Update progress and status text within lock
                        lock (progressLock)
                        {
                            Interlocked.Exchange(ref _progress, progress);
                            StatusText = BuildStatusText(processedItems, totalItems, progress);
                            Progress = progress;
                        }
                    }));

                    if (tasks.Count >= ThreadCount)
                    {
                        await Task.WhenAny(tasks);
                        tasks.RemoveAll(t => t.IsCompleted);
                    }
                }
            }

            await Task.WhenAll(tasks);

            StatusText = "Completed";
            IsActive = false;
        }


        protected abstract Task ProcessItemAsync(T item, CancellationToken token);

        protected virtual string BuildStatusText(int processedItems, int totalItems, float progress)
        {
            return $"Progress: {progress:F2}% ({processedItems}/{totalItems} items)";
        }

        private async Task<int> CountAsync(IAsyncEnumerable<T> asyncEnumerable)
        {
            int count = 0;
            await foreach (var _ in asyncEnumerable)
            {
                count++;
            }
            return count;
        }

        public void Reset()
        {
            Progress = 0;
            StatusText = "Not started";
            IsActive = false;
        }
    }
}
