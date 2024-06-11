using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using Fileshard.Frontend.Helpers;
using System.Diagnostics;

namespace Fileshard.Frontend.Components
{
    public class FileItem : INotifyPropertyChanged
    {
        private string name;
        private string path;
        private Guid objectGuid;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    /// <summary>
    /// Interaction logic for ObjectListBoxItem.xaml
    /// </summary>
    public partial class ObjectListBoxItem : UserControl
    {
        private String _path = "";

        public ObjectListBoxItem()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private async void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FileItem fileItem)
            {
                fileItem.PropertyChanged += OnFileItemPropertyChanged;
                await StartDrawingTaskAsync(fileItem.Path);
            }
        }

        private void OpenContainingMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", "/select," + _path);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open containing folder: {ex.Message}");
            }
        }

        private void OpenFileMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = _path,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open file: {ex.Message}");
            }
        }

        private async void OnFileItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FileItem.Path) && sender is FileItem fileItem)
            {
                await StartDrawingTaskAsync(fileItem.Path);
            }
        }

        private async Task StartDrawingTaskAsync(string path)
        {
            try
            {
                var thumbnail = await Task.Run(() => ThumbnailUtil.GetThumbnailByPath(path));
                IconImage.Source = thumbnail;
                _path = path;
            }
            catch (Exception e)
            {
                // Handle exception
            }
        }
    }
}
