using SkiaSharp;
using System.Windows;
using System.IO;
using Fileshard.Frontend.Helpers;
using System.Windows.Media;
using ServiceWire.NamedPipes;
using Fileshard.Shared.IPC;
using Fileshard.Shared.Structs;

namespace Fileshard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Database> _databases;

        public MainWindow()
        {
            InitializeComponent();


            using (var client = new NpClient<DatabaseIPC>(new NpEndPoint("fileshard")))
            {
                _databases = client.Proxy.GetDatabases();
                _databases.ForEach(db => this.StatusTextBlock.Text += $"{db.Guid}\n");
            }
        }

        private void DropGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string file in files)
                {
                    using var inputStream = File.OpenRead(file);
                    var bitmap = SKBitmap.Decode(inputStream);

                    skiaImageViewer.LoadBitmap(bitmap); 
                    this.StatusTextBlock.Text = $"Loaded {Path.GetFileName(file)}";
                }
            }
        }
    }
}