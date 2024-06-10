using System.IO;
using System.Windows.Media.Imaging;
using ImageMagick;

namespace Fileshard.Frontend.Helpers
{
    internal class ThumbnailUtil
    {
        private static readonly SemaphoreSlim semaphore = new SemaphoreSlim(1);
        private static readonly string cacheDirectory = Path.Combine(Path.GetTempPath(), "ThumbnailCache");

        static ThumbnailUtil()
        {
            Directory.CreateDirectory(cacheDirectory);
        }

        public static BitmapImage GetThumbnailByPath(string path)
        {
            string cachePath = Path.Combine(cacheDirectory, $"{Path.GetFileName(path)}.png");

            try
            {
                semaphore.Wait();

                if (File.Exists(cachePath))
                {
                    return LoadImageFromPath(cachePath);
                }

                return CreateAndCacheThumbnail(path, cachePath);
            }
            catch (Exception)
            {
                return GetBlankImage();
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static BitmapImage LoadImageFromPath(string imagePath)
        {
            using var fileStream = File.OpenRead(imagePath);
            return LoadImageFromStream(fileStream);
        }

        private static BitmapImage LoadImageFromStream(Stream stream)
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.StreamSource = stream;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            return bitmapImage;
        }

        private static BitmapImage GetBlankImage()
        {
            using var blankImage = new MagickImage(MagickColors.Transparent, 100, 100);
            using var memoryStream = new MemoryStream();
            blankImage.Write(memoryStream, MagickFormat.Png);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return LoadImageFromStream(memoryStream);
        }

        private static BitmapImage CreateAndCacheThumbnail(string imagePath, string cachePath)
        {
            using var image = new MagickImage(imagePath);

            image.Resize(new MagickGeometry(100, 100) { IgnoreAspectRatio = false });

            using var canvas = new MagickImage(MagickColors.White, 100, 100);

            int x = (canvas.Width - image.Width) / 2;
            int y = (canvas.Height - image.Height) / 2;

            canvas.Composite(image, x, y, CompositeOperator.Over);

            using var memoryStream = new MemoryStream();
            canvas.Write(memoryStream, MagickFormat.Png);

            File.WriteAllBytes(cachePath, memoryStream.ToArray());
            memoryStream.Seek(0, SeekOrigin.Begin);

            return LoadImageFromStream(memoryStream);
        }
    }
}
