using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Process_Explorer.GUI.Helpers
{
    public static class IconHelper
    {
        public static async Task<BitmapImage> GetIconAsync(string exePath)
        {
            if (!File.Exists(exePath))
                return null!;

            try
            {
                Icon icon = Icon.ExtractAssociatedIcon(exePath)!;
                if (icon is null) return null!;

                using var memStream = new MemoryStream();
                icon.ToBitmap().Save(memStream, System.Drawing.Imaging.ImageFormat.Png);
                memStream.Seek(0, SeekOrigin.Begin);

                var bitmap = new BitmapImage();
                await bitmap.SetSourceAsync(memStream.AsRandomAccessStream());
                return bitmap;
            }
            catch
            {
                return null!;
            }
        }
        public static async Task<BitmapImage> GetDefaultIcon()
        {
            using (var bitmap = SystemIcons.Application.ToBitmap())
            {
                var bitmapImage = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage();
                using (var memoryStream = new System.IO.MemoryStream())
                {
                    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                    memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                    await bitmapImage.SetSourceAsync(memoryStream.AsRandomAccessStream());
                }
                return bitmapImage;
            }
        }
    }
}
