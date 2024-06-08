using ExifLibrary;
using Microsoft.Maui.Graphics.Platform;

namespace FireEscape.Common;

public static class ImageUtils
{
    public const string IMAGE_FILE_EXTENSION = "jpg";

    public static async Task TransformImageAsync(FileResult imageFile, string destinationFilePath, int maxImageSize = 0, float imageQuality = 1)
    {
        var fileInfo = new FileInfo(imageFile.FullPath);
        if (fileInfo.Length < 1024 * 1024) // save image file direct if size less 1 mB
        {
            fileInfo.CopyTo(destinationFilePath);
            return;
        }
        using var imageStream = await imageFile.OpenReadAsync();
        using var image = PlatformImage.FromStream(imageStream);
        var scale = maxImageSize == 0 ? 1 : (image.Height > image.Width ? image.Height : image.Width) / maxImageSize;
        using var resizedImage = scale <= 1 ? image : image.Resize(image.Width / scale, image.Height / scale, ResizeMode.Stretch, false);
        using var outputFile = File.Create(destinationFilePath);
        await resizedImage.SaveAsync(outputFile, ImageFormat.Jpeg, scale < .5f ? 1 : imageQuality);
    }

    public static ExifEnumProperty<Orientation>? GetImageOrientation(string filePath)
    {
        try
        {
            var file = ImageFile.FromFile(filePath);
            return file.Properties.Get<ExifEnumProperty<Orientation>>(ExifTag.Orientation);
        }
        catch
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
            angle = orientation.Value switch
            {
                Orientation.RotatedLeft => -90,
                Orientation.RotatedRight => 90,
                Orientation.Rotated180 => 180,
                _ => 0,
            };
        return angle * Math.PI / 180;
    }
}
