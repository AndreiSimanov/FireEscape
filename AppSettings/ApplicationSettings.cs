using CommunityToolkit.Maui.Storage;

namespace FireEscape.AppSettings;

public class ApplicationSettings
{
    const string IMAGES_FOLDER = "/Images";
    const string DOCUMENTS_FOLDER = "/Documents";
    const string LOG_FOLDER = "/Log";
    const string OUTPUT_FOLDER = "OutputFolder";

    public required string UserAccountsFolderName { get; set; }
    public int CheckUserAccountCounter { get; set; }
    public int MaxImageSize { get; set; }
    public float ImageQuality { get; set; }
    public int PageSize { get; set; }
    public required string LightThemeColor { get; set; }
    public required string DarkThemeColor { get; set; }
    public required string DbName { get; set; }
    public required UnitOfMeasure PrimaryUnitOfMeasure { get; set; }
    public required UnitOfMeasure SecondaryUnitOfMeasure { get; set; }

    public static string ImagesFolder => AppUtils.CreateFolderIfNotExists(AppUtils.DefaultContentFolder, IMAGES_FOLDER);
    public static string DocumentsFolder => AppUtils.CreateFolderIfNotExists(AppUtils.DefaultContentFolder, DOCUMENTS_FOLDER);
    public static string LogFolder => AppUtils.CreateFolderIfNotExists(AppUtils.DefaultContentFolder, LOG_FOLDER);
    public static string CacheFolder => FileSystem.CacheDirectory;

    /*
    public static async Task<string> GetOutputPath()
    {
        var path = Preferences.Get(OUTPUT_FOLDER, string.Empty);
        if (string.IsNullOrWhiteSpace(path) || !Directory.Exists(path))
        {
            var folderPickerResult = await FolderPicker.PickAsync(default); // Permission request must be invoked on main thread.
            path = folderPickerResult.IsSuccessful ? folderPickerResult.Folder.Path : string.Empty;
            if (!string.IsNullOrWhiteSpace(path))
                Preferences.Set(OUTPUT_FOLDER, path);
        }
        return path;
    }
    */

    public static void ClearOutputPathPreferences() => Preferences.Remove(OUTPUT_FOLDER);
}