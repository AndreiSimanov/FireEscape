using System.Text;

namespace FireEscape.Services;

public class ReportService(UserAccountService userAccountService,  IReportRepository reportRepository)
{
    public async Task CreateSingleReportAsync(Order order, Protocol protocol )
    {
        var folderPath = PrepareOutputFolder(order);
        if (string.IsNullOrWhiteSpace(folderPath))
            return;

        var userAccount = await userAccountService.GetCurrentUserAccountAsync();
        CheckUserAccount(userAccount);
        var outputPath = Path.Combine(folderPath, GetFileName(order, protocol));
        await reportRepository.CreateReportAsync(order, protocol, outputPath);
        userAccountService.UpdateExpirationCount(userAccount!);
        await Launcher.OpenAsync(new OpenFileRequest { File = new ReadOnlyFile(outputPath) });
    }

    public async Task CreateBatchReportAsync(Order order, Protocol[] protocols, IProgress<(double progress, string outputPath)>? progress, CancellationToken ct)
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
            await reportRepository.CreateReportAsync(order, protocol, outputPath);
            userAccountService.UpdateExpirationCount(userAccount!);
            progress?.Report((++count / protocols.Length, outputPath));
            if (ct.IsCancellationRequested)
                break;
            await Task.Yield();
        }
    }

    void CheckUserAccount(UserAccount? userAccount)
    {
        if (!UserAccountService.IsValidUserAccount(userAccount))
            throw new Exception(string.Format(AppResources.UnregisteredApplicationMessage, userAccountService.CurrentUserAccountId));
    }

    static string PrepareOutputFolder(Order order)
    {
        var outputPath = ApplicationSettings.DocumentsFolder;
        var defaultOrderFolderName = $"{AppResources.Order}_{order.Id}.";
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
        sb.Append(protocol.Stairs.BaseStairsType == BaseStairsTypeEnum.P1 ? "Верт" : "Марш");
        sb.Append(' ');
        if (protocol.Stairs.IsEvacuation)
        {
            sb.Append("эвакуац");
            sb.Append(' ');
        }
        sb.Append("лес");
        sb.Append(' ');
        sb.Append(protocol.Stairs.StairsMountType == StairsMountTypeEnum.BuildingMounted ? "на здание" : "на перепаде");
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
}