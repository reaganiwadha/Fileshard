using SkiaSharp.Views.Desktop;
using SkiaSharp;
using System.Windows;
using System.IO;
using Fileshard.Frontend.Helpers;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Fileshard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SKBitmap bitmap;


        [DllImport("DwmApi")]
        private static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, int[] attrValue, int attrSize);
        const int DWWMA_CAPTION_COLOR = 35;


        public MainWindow()
        {
            IntPtr hWnd = new WindowInteropHelper(this).EnsureHandle();
            int[] colorstr = new int[] { 0x000000 };
            DwmSetWindowAttribute(hWnd, DWWMA_CAPTION_COLOR, colorstr, 4);

            InitializeComponent();
            ApplyTheme();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (bitmap == null)
            {
                return;
            }

            var canvas = e.Surface.Canvas;

            bool isDarkTheme = ThemeHelper.IsDarkTheme();
            canvas.Clear(isDarkTheme ? SKColors.Black : SKColors.White);

            // Create paint object with antialiasing enabled
            var paint = new SKPaint
            {
                IsAntialias = false,
                FilterQuality = SKFilterQuality.High // Set the filter quality to high for smoother scaling
            };

            // Calculate the scaling factor to fit the image within the SKElement
            float scale = Math.Min((float)e.Info.Width / bitmap.Width, (float)e.Info.Height / bitmap.Height);

            // Calculate the image dimensions after scaling
            float scaledWidth = scale * bitmap.Width;
            float scaledHeight = scale * bitmap.Height;

            // Center the image in the SKElement
            float x = (e.Info.Width - scaledWidth) / 2;
            float y = (e.Info.Height - scaledHeight) / 2;   

            // Define the destination rectangle
            var destRect = new SKRect(x, y, x + scaledWidth, y + scaledHeight);

            // Draw bitmap with smooth antialiasing
            canvas.DrawBitmap(bitmap, destRect, paint);
        }


        private void ApplyTheme()
        {
            bool isDarkTheme = ThemeHelper.IsDarkTheme();
            var backgroundColor = isDarkTheme ? Colors.Black : Colors.White;
            var foregroundColor = isDarkTheme ? Colors.White : Colors.Black;

            // Apply the theme to the window
            this.Background = new SolidColorBrush(backgroundColor);
            this.Foreground = new SolidColorBrush(foregroundColor);
        }

            private void DropGrid_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Get the file(s) that were dropped
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Process the files
                foreach (string file in files)
                {
                    using var inputStream = File.OpenRead(file);
                    bitmap = SKBitmap.Decode(inputStream);

                    skElement.InvalidateVisual();
                }
            }
        }
    }
}