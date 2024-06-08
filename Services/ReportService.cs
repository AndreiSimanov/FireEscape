using System.IO.Compression;
using System.Text;

namespace FireEscape.Services;

public class ReportService(UserAccountService userAccountService, IReportRepository reportRepository)
{
    public async Task CreateSingleReportAsync(Order order, Protocol protocol)
    {
        var folderPath = PrepareOutputFolder(order);
        if (string.IsNullOrWhiteSpace(folderPath))
            return;

        var userAccount = await userAccountService.GetCurrentUserAccountAsync();
        CheckUserAccount(userAccount);
        var outputPath = Path.Combine(folderPath, GetFileName(order, protocol));
        outputPath = IncrementFileNameIfExists(outputPath);
        await reportRepository.CreateReportAsync(order, protocol, outputPath);
        userAccountService.UpdateExpirationCount(userAccount!);
        await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(outputPath) });
    }

    public async Task CreateBatchReportAsync(Order order, Protocol[] protocols, CancellationToken ct, IProgress<(double progress, string outputPath)>? progress = null)
    {
        var folderPath = PrepareOutputFolder(order);
        if (string.IsNullOrWhiteSpace(folderPath))
            return;
        var userAccount = await userAccountService.GetCurrentUserAccountAsync();
        CheckUserAccount(userAccount);
        AppUtils.DeleteFolderContent(folderPath);
        double count = 0;
        foreach (var protocol in protocols)
        {
            var outputPath = Path.Combine(folderPath, GetFileName(order, protocol));
            outputPath = IncrementFileNameIfExists(outputPath);
            await reportRepository.CreateReportAsync(order, protocol, outputPath);
            userAccountService.UpdateExpirationCount(userAccount!);
            progress?.Report((++count / protocols.Length, outputPath));
            if (ct.IsCancellationRequested)
                break;
            await Task.Yield();
        }
    }

    public static IEnumerable<FileInfo> GetReports(Order order) => new DirectoryInfo(PrepareOutputFolder(order)).EnumerateFiles();

    public static async Task MakeReportArchiveAsync(ICollection<FileInfo> files, CancellationToken ct, IProgress<double>? progress = null)
    {
        if (files.Count == 0)
            return;

        using var archiveStream = await ReportService.GetArchiveStream(files, ct, progress);

        if (ct.IsCancellationRequested)
            return;

        var archiveFileName = files.FirstOrDefault()!.Directory!.Name;
        if (string.IsNullOrWhiteSpace(archiveFileName))
            archiveFileName = AppResources.Order;

        archiveFileName += archiveFileName.EndsWith('.') ? "zip" : ".zip";

        var archiveFilePath = Path.Combine(ApplicationSettings.CacheFolder, archiveFileName);

        await using (var fileStream = new FileStream(archiveFilePath, FileMode.OpenOrCreate))
        {
            await archiveStream.CopyToAsync(fileStream);
        }

        await Share.RequestAsync(new ShareFileRequest
        {
            Title = AppResources.SharingOrderZip,
            File = new ShareFile(archiveFilePath)
        });
    }

    static async Task<MemoryStream> GetArchiveStream(ICollection<FileInfo> files, CancellationToken ct, IProgress<double>? progress = null)
    {
        var ms = new MemoryStream();
        double count = 0;
        using (var a = new ZipArchive(ms, ZipArchiveMode.Create, true))
        {
            foreach (var fileInfo in files)
            {
                a.CreateEntryFromFile(fileInfo.FullName, fileInfo.Name);
                progress?.Report(++count / files.Count);
                if (ct.IsCancellationRequested)
                    break;
                await Task.Yield();
            }
        }
        ms.Position = 0;
        return ms;
    }

    void CheckUserAccount(UserAccount? userAccount)
    {
        if (!UserAccountService.IsValidUserAccount(userAccount))
            throw new Exception(string.Format(AppResources.UnregisteredApplicationMessage, userAccountService.CurrentUserAccountId));
    }

    static string PrepareOutputFolder(Order order)
    {
        var outputPath = ApplicationSettings.DocumentsFolder;
        var defaultOrderFolderName = $"{AppResources.Order}_{order.Id}_";
        var orderFolderName = defaultOrderFolderName + (string.IsNullOrWhiteSpace(order.Name) ? string.Empty : AppUtils.ToValidFileName(order.Name.Trim()));
        var fullPath = Path.Combine(outputPath, orderFolderName);

        var folders = Directory.GetDirectories(outputPath, defaultOrderFolderName + '*');
        if (folders.Length > 0)
        {
            if (string.Equals(folders[0], fullPath))
                return fullPath;
            Directory.Move(folders[0], fullPath);
            return fullPath;
        }
        return AppUtils.CreateFolderIfNotExists(fullPath);
    }

    static string GetFileName(Order order, Protocol protocol)
    {
        var fireEscapeObject = string.IsNullOrWhiteSpace(protocol.FireEscapeObject) ? order.FireEscapeObject : protocol.FireEscapeObject;
        if (string.IsNullOrWhiteSpace(fireEscapeObject))
            fireEscapeObject = string.IsNullOrWhiteSpace(protocol.Address) ? order.Address : protocol.Address;

        var sb = new StringBuilder();
        sb.Append(protocol.FireEscapeNum);
        sb.Append('.');
        sb.Append(protocol.Stairs.BaseStairsType == BaseStairsTypeEnum.P1 ? AppResources.P1Trim : AppResources.P2Trim);
        sb.Append(' ');
        if (protocol.Stairs.IsEvacuation)
        {
            sb.Append(AppResources.EscapeStairsTrim);
            sb.Append(' ');
        }
        sb.Append(AppResources.StairsTrim);
        sb.Append(' ');
        sb.Append(protocol.Stairs.StairsMountType == StairsMountTypeEnum.BuildingMounted ? AppResources.BuildingMountedTrim : AppResources.ElevationMountedTrim);
        sb.Append(" №");
        sb.Append(protocol.FireEscapeNum);
        sb.Append('.');
        if (!string.IsNullOrWhiteSpace(fireEscapeObject))
        {
            sb.Append(' ');
            sb.Append(AppUtils.ToValidFileName(fireEscapeObject));
            sb.Append('.');
        }
        sb.Append("pdf");
        return sb.ToString();
    }

    static string IncrementFileNameIfExists(string filePath)
    {
        if (!File.Exists(filePath))
            return filePath;

        var path = Path.GetDirectoryName(filePath);
        var fileName = Path.GetFileNameWithoutExtension(filePath);
        var fileExt = Path.GetExtension(filePath);
        var counter = 2;

        while (File.Exists(filePath))
        {
            filePath = Path.Combine(path!, $"{fileName}({counter++}){fileExt}");
        }
        return filePath;
    }
}