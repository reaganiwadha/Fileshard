using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fileshard.Frontend.Helpers
{
    public class BitmapScaler
    {
        private SKBitmap _bitmap;

        public BitmapScaler(SKBitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public SKRect CalculateDestinationRect(SKSizeI surfaceSize)
        {
            float scale = Math.Min((float)surfaceSize.Width / _bitmap.Width, (float)surfaceSize.Height / _bitmap.Height);

            float scaledWidth = scale * _bitmap.Width;
            float scaledHeight = scale * _bitmap.Height;

            float x = (surfaceSize.Width - scaledWidth) / 2;
            float y = (surfaceSize.Height - scaledHeight) / 2;

            return new SKRect(x, y, x + scaledWidth, y + scaledHeight);
        }
    }

}
