using System.Diagnostics;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FireEscape.Common;

public static class AppUtils
{
    const string MULTIPLE_SPACES_PATTERN = @"([ ])\1+";

    public static async Task<string> GetExternalContentFolderAsync(string applicationFolderName)
    {
#if ANDROID
        await MainActivity.AllFilesAccessPermissionTask;
        if (OperatingSystem.IsAndroidVersionAtLeast(30) &&
            Android.OS.Environment.ExternalStorageDirectory != null &&
            Android.OS.Environment.IsExternalStorageManager)
        {
            return CreateFolderIfNotExists(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath, applicationFolderName);
        }

        return DefaultContentFolder;
#else
        await Task.FromResult(true);
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
#endif

    }

    public static string DefaultContentFolder
    {
        get
        {

#if ANDROID
            var docsDirectory = Android.App.Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDocuments);
            return (docsDirectory!.AbsoluteFile.Parent == null) ? docsDirectory!.AbsoluteFile.Path : docsDirectory!.AbsoluteFile.Parent;
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

    public static string ToValidFileName(string fileName) =>
         Regex.Replace(GetInvalidFileNameChars().Aggregate(fileName, (f, c) => f.Replace(c, ' ')), MULTIPLE_SPACES_PATTERN, " ").Trim();

    public static char[] GetInvalidFileNameChars() =>
        [
            '\"', '<', '>', '|', '\0',
            (char)1, (char)2, (char)3, (char)4, (char)5, (char)6, (char)7, (char)8, (char)9, (char)10,
            (char)11, (char)12, (char)13, (char)14, (char)15, (char)16, (char)17, (char)18, (char)19, (char)20,
            (char)21, (char)22, (char)23, (char)24, (char)25, (char)26, (char)27, (char)28, (char)29, (char)30,
            (char)31, ':', '*', '?', '\\', '/'
        ];

    public static string CreateFolderIfNotExists(string path, string folderName = "")
    {
        path = Path.Join(path, folderName);
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

        foreach (var file in di.EnumerateFiles())
            file.Delete();

        if (recursive)
            foreach (var dir in di.EnumerateDirectories())
                dir.Delete(true);
    }

    public static bool TryDeserialize<T>(string json, out T? result)
    {
        result = default;
        if (string.IsNullOrWhiteSpace(json))
            return false;
         
        try
        {
            result = JsonSerializer.Deserialize<T>(json);
        }
        catch
        {
            return false;
        }
        return result != null;
    }

    public static async void SafeFireAndForget(this Task task,  Action<Exception>? onException)
    {
        try
        {
            await task.ConfigureAwait(false);
        }
        catch (Exception ex) 
        {
            Debug.WriteLine(ex);
            onException?.Invoke(ex);
        }
    }
}
