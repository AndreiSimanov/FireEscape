using ExifLibrary;
using Microsoft.Maui.Graphics.Platform;

namespace FireEscape.Common
{
    public static class ImageUtils
    {
        public const string IMAGE_FILE_EXTENSION = "jpg";

        public static async Task TransformImage(FileResult imageFile, int maxImageSize, string destinationFilePath)
        {
            if (maxImageSize == 0)
                throw new ArgumentOutOfRangeException(nameof(maxImageSize));
            using var imageStream = await imageFile.OpenReadAsync();
            using var image = PlatformImage.FromStream(imageStream);
            var scale = (image.Height > image.Width ? image.Height : image.Width) / maxImageSize;
            using var resizedImage = image.Resize(image.Width / scale, image.Height / scale, ResizeMode.Stretch, false);
            using var outputFile = File.Create(destinationFilePath);
            await resizedImage.SaveAsync(outputFile, ImageFormat.Jpeg);
        }

        public static ExifEnumProperty<Orientation>? GetImageOrientation(string filePath)
        {
            try
            {
                var file = ImageFile.FromFile(filePath);
                return file.Properties.Get<ExifEnumProperty<Orientation>>(ExifTag.Orientation);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void SetImageOrientation(string filename, ExifEnumProperty<Orientation>? orientation)
        {
            if (orientation == null)
                return;
            var file = ImageFile.FromFile(filename);
            file.Properties.Set<Orientation>(ExifTag.Orientation, orientation);
            file.Save(filename);
        }

        public static double GetRotation(string filePath)
        {
            var orientation = GetImageOrientation(filePath);
            int angle = 0;
            if (orientation != null)
                switch (orientation.Value)
                {
                    case Orientation.RotatedLeft:
                        angle = -90;
                        break;
                    case Orientation.RotatedRight:
                        angle = 90;
                        break;
                    case Orientation.Rotated180:
                        angle = 180;
                        break;
                    default:
                        angle = 0;
                        break;
                }
            return angle * Math.PI / 180;
        }
    }
}
