using SkiaSharp.Views.Desktop;
using SkiaSharp;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Windows;   
using Fileshard.Frontend.Helpers;

namespace Fileshard.Frontend.Components
{
    public class FileItem : INotifyPropertyChanged
    {
        private string name;
        private string path;
        private Guid objectGuid;
        private BitmapImage icon;

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        public Guid ObjectGuid
        {
            get => objectGuid;
            set
            {
                if (objectGuid != value)
                {
                    objectGuid = value;
                    OnPropertyChanged(nameof(ObjectGuid));
                }
            }
        }

        public string Path
        {
            get => path;
            set
            {
                if (path != value)
                {
                    path = value;
                    OnPropertyChanged(nameof(Path));
                }
            }
        }

        public BitmapImage Icon
        {
            get => icon;
            set
            {
                if (icon != value)
                {
                    icon = value;
                    OnPropertyChanged(nameof(Icon));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class DrawingMutex
    {
        private static DrawingMutex _instance;
        private static readonly object _lock = new object();
        private readonly Mutex _mutex;

        private DrawingMutex()
        {
            _mutex = new Mutex();
        }

        public static DrawingMutex Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new DrawingMutex();
                        }
                    }
                }
                return _instance;
            }
        }

        public void WaitOne()
        {
            _mutex.WaitOne();
        }

        public void ReleaseMutex()
        {
            _mutex.ReleaseMutex();
        }
    }

    /// <summary>
    /// Interaction logic for ObjectListBoxItem.xaml
    /// </summary>
    public partial class ObjectListBoxItem : UserControl
    {
        public ObjectListBoxItem()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FileItem fileItem)
            {
                fileItem.PropertyChanged += OnFileItemPropertyChanged;
                StartDrawingTask(fileItem.Path);
            }

            if (e.OldValue is FileItem oldFileItem)
            {
                oldFileItem.PropertyChanged -= OnFileItemPropertyChanged;
            }
        }

        private void OnFileItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileItem.Path) && sender is FileItem fileItem)
            {
                StartDrawingTask(fileItem.Path);
            }
        }

        Boolean isDrawingTurn = false;

        private void StartDrawingTask(string path)
        {
            Task.Run(() =>
            {
                DrawingMutex.Instance.WaitOne();

                try { 
                    isDrawingTurn = true;
                    Application.Current.Dispatcher.Invoke(() => {
                        skElement.InvalidateVisual();
                    });
                } finally
                {
                    Thread.Sleep(50);
                    DrawingMutex.Instance.ReleaseMutex();
                }
            });
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (isDrawingTurn == false) return;

            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear(SKColors.White);

            if (DataContext is FileItem fileItem && !string.IsNullOrEmpty(fileItem.Path))
            {
                try
                {
                    using (var stream = new SKFileStream(fileItem.Path))
                    using (var bitmap = SKBitmap.Decode(stream))
                    {
                        if (bitmap != null)
                        {
                            var paint = new SKPaint
                            {
                                IsAntialias = false,
                                FilterQuality = SKFilterQuality.High
                            };

                            canvas.Clear(SKColors.White);

                            var scaler = new BitmapScaler(bitmap);
                            var destRect = scaler.CalculateDestinationRect(e.Info.Size);

                            canvas.DrawBitmap(bitmap, destRect, paint);
                            bitmap.Dispose();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
