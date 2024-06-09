using SkiaSharp;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Fileshard.Frontend.Components;
using Fileshard.Service.Repository;
using Fileshard.Service.Database;

namespace Fileshard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<FileItem> Files { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        private readonly ICollectionRepository collectionRepository = new CollectionRepository();

        public MainWindow()
        {
            InitializeComponent();

            collectionRepository.IsEmpty().ContinueWith(task =>
            {
                if (task.Result)
                {
                    Dispatcher.Invoke(() =>
                    {
                       this.StatusTextBlock.Text = "No collections found... Please setup";
                    });
                }
            });

            Files = new ObservableCollection<FileItem>
            {
            };

            DataContext = this;
        }

        private void DropGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    string extension = Path.GetExtension(file).ToLower();

                    if (extension == ".jpg" || extension == ".png") { 
                        Files.Add(new FileItem { Name = Path.GetFileName(file), Path = file, Icon = null });
                        this.StatusTextBlock.Text = $"Loaded {Path.GetFileName(file)}";
                    } 
                    else
                    {
                        this.StatusTextBlock.Text = $"Unsupported file format: {Path.GetFileName(file)}";
                    }
                }
            }
        }

        private void LoadFile(string file)
        {
            var inputStream = SKData.Create(file);
            skiaImageViewer.LoadBitmap(inputStream);
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListBox.SelectedItem != null)
            {
                FileItem selectedFile = (FileItem)ListBox.SelectedItem;
                LoadFile(selectedFile.Path);
            }
        }
    }
}
