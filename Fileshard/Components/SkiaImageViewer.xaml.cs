using SkiaSharp.Views.Desktop;
using SkiaSharp;
using System.Windows.Controls;

namespace Fileshard.Frontend.Components
{
    public partial class SkiaImageViewer : UserControl
    {
        private SKBitmap? _skBitmap;

        public SkiaImageViewer()
        {
            InitializeComponent();
        }

        public void LoadBitmap(SKData sKData)
        {
            try { 
            if (this._skBitmap != null)
            {
                this._skBitmap.Dispose();
            }

            var bitmap = SKBitmap.Decode(sKData);
            sKData.Dispose();

            this._skBitmap = bitmap;
            skElement.InvalidateVisual();
            } catch (System.Exception e)
            {
                System.Windows.MessageBox.Show("Error while drawing image " + e.Message);
            }
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_skBitmap == null) return;

            var canvas = e.Surface.Canvas;

            var paint = new SKPaint
            {
                IsAntialias = false,
                FilterQuality = SKFilterQuality.High 
            };

            canvas.Clear(SKColors.White);

            float scale = Math.Min((float)e.Info.Width / _skBitmap.Width, (float)e.Info.Height / _skBitmap.Height);

            float scaledWidth = scale * _skBitmap.Width;
            float scaledHeight = scale * _skBitmap.Height;

            float x = (e.Info.Width - scaledWidth) / 2;
            float y = (e.Info.Height - scaledHeight) / 2;

            var destRect = new SKRect(x, y, x + scaledWidth, y + scaledHeight);

            canvas.DrawBitmap(_skBitmap, destRect, paint);
        }
    }
}
