namespace FireEscape.AppSettings;

public class ApplicationSettings
{
    const string IMAGES_FOLDER = "/Images";
    const string DOCUMENTS_FOLDER = "/Documents";
    const string LOG_FOLDER = "/Log";

    public required string UserAccountsFolderName { get; set; }
    public int CheckUserAccountCounter { get; set; }
    public int MaxImageSize { get; set; }
    public float ImageQuality { get; set; }
    public int PageSize { get; set; }
    public required string PrimaryThemeColor { get; set; }
    public required string DbName { get; set; }
    public required UnitOfMeasure PrimaryUnitOfMeasure { get; set; }
    public required UnitOfMeasure SecondaryUnitOfMeasure { get; set; }

    public static string ImagesFolder => CreateFolderIfNotExist(AppUtils.DefaultContentFolder, IMAGES_FOLDER);
    public static string DocumentsFolder => CreateFolderIfNotExist(AppUtils.DefaultContentFolder, DOCUMENTS_FOLDER);
    public static string LogFolder => CreateFolderIfNotExist(AppUtils.DefaultContentFolder, LOG_FOLDER);

    static string CreateFolderIfNotExist(string path, string folderName = "" )
    {
        path = Path.Combine(Path.Join(path, folderName));
        if (Directory.Exists(path))
            return path;
        var directoryInfo = Directory.CreateDirectory(path);
        return directoryInfo.FullName;
    }
}
