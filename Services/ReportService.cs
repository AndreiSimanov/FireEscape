using System.Text;

namespace FireEscape.Services;

public class ReportService(UserAccountService userAccountService,  IReportRepository reportRepository)
{
    public async Task CreateSingleReportAsync(Order order, Protocol protocol )
    {
        var folderPath = GetOutputFolderPath(order);
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
        var folderPath = GetOutputFolderPath(order);
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

    static string GetOutputFolderPath(Order order)
    {
        var outputPath = ApplicationSettings.DocumentsFolder;
        if (string.IsNullOrWhiteSpace(outputPath))
            return string.Empty;

        var orderFolderName = AppResources.Order + "_" + order.Id;
        if (!string.IsNullOrWhiteSpace(order.Name))
            orderFolderName = orderFolderName + "_" + AppUtils.ToValidFileName(order.Name.Trim());

        return AppUtils.CreateFolderIfNotExists(outputPath, orderFolderName);
    }

    static string GetFileName(Order order, Protocol protocol)
    {
        var fireEscapeObject = string.IsNullOrWhiteSpace(protocol.FireEscapeObject) ? order.FireEscapeObject : protocol.FireEscapeObject;
        if (string.IsNullOrWhiteSpace(fireEscapeObject))
            fireEscapeObject = string.IsNullOrWhiteSpace(protocol.Address) ? order.Address : protocol.Address;

        var sb = new StringBuilder();
        sb.Append(protocol.StairsId);
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
        sb.Append(protocol.StairsId);
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