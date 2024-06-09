using SkiaSharp;
using System.Windows;
using System.Windows.Controls;
using Fileshard.Frontend.Components;
using Fileshard.Frontend;

namespace Fileshard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;
        }

        private void DropGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

              _viewModel.OnFileDropped(files.ToList());
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
                _viewModel.OnObjectSelected(selectedFile.ObjectGuid);
                LoadFile(selectedFile.Path);
            }
        }

        private async void CreateNewCollection_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new NewCollectionDialog();
            if (dialog.ShowDialog() != true) return;

            _viewModel.CreateAndSelectCollection(dialog.ResponseText);
        }
    }
}
