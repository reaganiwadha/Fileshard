using SkiaSharp.Views.Desktop;
using SkiaSharp;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Fileshard.Frontend.Components
{
    public class FileItem
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public BitmapImage Icon { get; set; }
    }

    /// <summary>
    /// Interaction logic for ObjectListBoxItem.xaml
    /// </summary>
    public partial class ObjectListBoxItem : UserControl
    {
        Random rnd = new Random();

        public FileItem Item { get; set; }

        public ObjectListBoxItem()
        {
            InitializeComponent();
            DataContext = this;
            // skElement.InvalidateVisual();
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;


            canvas.Clear(SKColors.White);

            // Your SkiaSharp drawing code here
            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Blue,
                IsAntialias = true
            };

            // Drawing the path text
            if (Item != null && !string.IsNullOrEmpty(Item.Path))
            {
                SKPaint textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    IsAntialias = true,
                    TextAlign = SKTextAlign.Center,
                    TextSize = 16
                };
                // Calculate text bounds
                SKRect textBounds = new SKRect();
                textPaint.MeasureText(Item.Path, ref textBounds);

                // Calculate text position
                float x = e.Info.Width / 2;
                float y = e.Info.Height / 2;

                // Draw text
                canvas.DrawText(Item.Path, 0, 0, textPaint);
            }

            canvas.DrawCircle(
                rnd.Next(1, 50),
                rnd.Next(1, 50),
                rnd.Next(1, 50), 
            paint);
        }
    }
}
