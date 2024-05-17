namespace FireEscape.AppSettings;

public class ApplicationSettings
{
    const string IMAGES_FOLDER = "/Images";
    const string DOCUMENTS_FOLDER = "/Documents";

    public required string UserAccountsFolderName { get; set; }
    public int CheckUserAccountCounter { get; set; }
    public int MaxImageSize { get; set; }
    public float ImageQuality { get; set; }
    public int PageSize { get; set; }
    public required string PrimaryThemeColor { get; set; }
    public required string DbName { get; set; }
    string contentFolder = string.Empty;
    public required string ContentFolder
    {
        get
        {
            if (string.IsNullOrWhiteSpace(contentFolder))
                return AppUtils.DefaultContentFolder;
            return CreateFolderIfNotExist(contentFolder);
        }
        set => contentFolder = value;
    }
    public string ImagesFolder => CreateFolderIfNotExist(ContentFolder, IMAGES_FOLDER);
    public string DocumentsFolder => CreateFolderIfNotExist(ContentFolder, DOCUMENTS_FOLDER);

    static string CreateFolderIfNotExist(string path, string folderName = "" )
    {
        path = Path.Combine(Path.Join(path, folderName));
        if (Directory.Exists(path))
            return path;
        var directoryInfo = Directory.CreateDirectory(path);
        return directoryInfo.FullName;
    }
}
