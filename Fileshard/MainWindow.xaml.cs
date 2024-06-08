using SkiaSharp;
using System.Windows;
using System.IO;
using Fileshard.Frontend.Helpers;
using System.Windows.Media;

namespace Fileshard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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