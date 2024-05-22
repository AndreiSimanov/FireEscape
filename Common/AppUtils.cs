namespace FireEscape.Common;

public static class AppUtils
{
    public static string DefaultContentFolder
    {
        get
        {

#if ANDROID
            var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
            return (docsDirectory!.AbsoluteFile.Parent == null) 
                ? docsDirectory!.AbsoluteFile.Path
                : docsDirectory!.AbsoluteFile.Parent;
#else
            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#endif
        }
    }

    public static async Task<bool> IsNetworkAccessAsync()
    {
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            return true;

        await Shell.Current.DisplayAlert(AppResources.NoConnectivity, AppResources.CheckInternetMessage, AppResources.OK);
        return false;
    }

    public static string ToValidFileName(string fileName) => Path.GetInvalidFileNameChars().Aggregate(fileName, (f, c) => f.Replace(c, '_'));

    public static string CreateFolderIfNotExist(string path, string folderName = "")
    {
        path = Path.Combine(Path.Join(path, folderName));
        if (Directory.Exists(path))
            return path;
        var directoryInfo = Directory.CreateDirectory(path);
        return directoryInfo.FullName;
    }

    public static void DeleteFolderContent(string path, bool recursive = false)
    {
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
            return;

        var di = new DirectoryInfo(path);

        di.EnumerateFiles().ToList().ForEach(file => file.Delete());

        if (recursive)
            di.EnumerateDirectories().ToList().ForEach(dir => dir.Delete(true));
    }
}
