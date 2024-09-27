using System.Text;

namespace FireEscape.Services;

public class ReportService(IUserAccountService userAccountService, IRemoteLogService remoteLogService, 
    IReportRepository reportRepository, ILogger<ReportService> logger) : IReportService
{
    public async Task CreateSingleReportAsync(Order order, Protocol protocol, bool incrementFileNameIfExists = false)
    {
        var folderPath = await PrepareOutputFolderAsync(order);
        if (string.IsNullOrWhiteSpace(folderPath))
            return;
        await userAccountService.CheckCurrentUserAsync();
        var outputPath = Path.Combine(folderPath, GetFileName(order, protocol));
        if (incrementFileNameIfExists)
            outputPath = IncrementFileNameIfExists(outputPath);
        await reportRepository.CreateReportAsync(order, protocol, outputPath);
        userAccountService.UpdateExpirationCount();
        await Launcher.OpenAsync(new OpenFileRequest { Title = AppResources.PdfView, File = new ReadOnlyFile(outputPath) });
    }

    public async Task CreateBatchReportAsync(Order order, Protocol[] protocols, CancellationToken ct, IProgress<(double progress, string outputPath)>? progress = null)
    {
        var folderPath = await PrepareOutputFolderAsync(order);
        if (string.IsNullOrWhiteSpace(folderPath))
            return;
        await userAccountService.CheckCurrentUserAsync();
        AppUtils.DeleteFolderContent(folderPath);
        AddRemoteLog(order, protocols);
        double count = 0;
        foreach (var protocol in protocols)
        {
            var outputPath = Path.Combine(folderPath, GetFileName(order, protocol));
            outputPath = IncrementFileNameIfExists(outputPath);
            await reportRepository.CreateReportAsync(order, protocol, outputPath);
            userAccountService.UpdateExpirationCount();
            progress?.Report((++count / protocols.Length, outputPath));
            if (ct.IsCancellationRequested)
                break;
            await Task.Yield();
        }
    }

    public async Task<IEnumerable<FileInfo>> GetReportsAsync(Order order) =>
        new DirectoryInfo(await PrepareOutputFolderAsync(order)).EnumerateFiles();

    void AddRemoteLog(Order order, Protocol[] protocols)
    {
        var message = $"{AppResources.Order}{AppResources.CaptionDivider} {order.Name}{Environment.NewLine}" +
            $"{AppResources.NumberOfProtocols}{AppResources.CaptionDivider} {protocols.Length}{Environment.NewLine}" +
            $"{AppResources.PrimaryExecutorSign}{AppResources.CaptionDivider} {order.PrimaryExecutorSign}{Environment.NewLine}" +
            $"{AppResources.SecondaryExecutorSign}{AppResources.CaptionDivider} {order.SecondaryExecutorSign}";

        remoteLogService.LogAsync(userAccountService.CurrentUserAccountId, RemoteLogCategoryType.BatchReport, message).SafeFireAndForget(ex => logger.LogError(ex, ex.Message));
    }

    static async Task<string> PrepareOutputFolderAsync(Order order)
    {
        var outputPath = await ApplicationSettings.GetDocumentsFolderAsync();
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