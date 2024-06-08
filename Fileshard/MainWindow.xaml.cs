using SkiaSharp;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ServiceWire.NamedPipes;
using Fileshard.Shared.IPC;
using Fileshard.Shared.Structs;
using System.Collections.ObjectModel;
using Fileshard.Frontend.Components;
using System.Windows.Controls.Primitives;

namespace Fileshard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Database> _databases;

        public ObservableCollection<FileItem> Files { get; set; }
        public int RowCount { get; set; }
        public int ColumnCount { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                using (var client = new NpClient<DatabaseIPC>(new NpEndPoint("fileshard")))
                {
                    _databases = client.Proxy.GetDatabases();
                    _databases.ForEach(db => this.StatusTextBlock.Text += $"{db.Guid}\n");
                }
            }
            catch (Exception e)
            {
                this.StatusTextBlock.Text = e.Message;
            }

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
