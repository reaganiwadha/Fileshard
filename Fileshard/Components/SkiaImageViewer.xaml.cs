using SkiaSharp.Views.Desktop;
using SkiaSharp;
using System.Windows.Controls;

namespace Fileshard.Frontend.Components
{
    public partial class SkiaImageViewer : UserControl
    {
        private SKBitmap bitmap;

        public SkiaImageViewer()
        {
            InitializeComponent();
        }

        public void LoadBitmap(SKBitmap newBitmap)
        {
            bitmap = newBitmap;
            skElement.InvalidateVisual();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (bitmap == null)
            {
                return;
            }

            var canvas = e.Surface.Canvas;

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
    }
}
