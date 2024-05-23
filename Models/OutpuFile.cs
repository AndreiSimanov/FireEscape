namespace FireEscape.Models;

public partial class OutpuFile : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FileName))]
    [NotifyPropertyChangedFor(nameof(FolderPath))]
    string filePath = string.Empty;
    public string FileName => string.IsNullOrWhiteSpace(FilePath) ? string.Empty : Path.GetFileName(FilePath);
    public string FolderPath => string.IsNullOrWhiteSpace(FilePath) ? string.Empty : FilePath.Substring(0, FilePath.IndexOf(FileName));
}
