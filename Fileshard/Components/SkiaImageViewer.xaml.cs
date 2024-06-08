using SkiaSharp.Views.Desktop;
using SkiaSharp;
using System.Windows.Controls;

namespace Fileshard.Frontend.Components
{
    public partial class SkiaImageViewer : UserControl
    {
        private SKData? _skData;

        public SkiaImageViewer()
        {
            InitializeComponent();
        }

        public void LoadBitmap(SKData sKData)
        {
            if (this._skData != null)
            {
                this._skData.Dispose();
            }

            this._skData = sKData;
            skElement.InvalidateVisual();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_skData == null)
            {
                return;
            }

            var bitmap = SKBitmap.Decode(_skData);

            var canvas = e.Surface.Canvas;

            var paint = new SKPaint
            {
                IsAntialias = false,
                FilterQuality = SKFilterQuality.High 
            };

            canvas.Clear(SKColors.White);

            float scale = Math.Min((float)e.Info.Width / bitmap.Width, (float)e.Info.Height / bitmap.Height);

            float scaledWidth = scale * bitmap.Width;
            float scaledHeight = scale * bitmap.Height;

            float x = (e.Info.Width - scaledWidth) / 2;
            float y = (e.Info.Height - scaledHeight) / 2;

            var destRect = new SKRect(x, y, x + scaledWidth, y + scaledHeight);

            canvas.DrawBitmap(bitmap, destRect, paint);
            bitmap.Dispose();
        }
    }
}
