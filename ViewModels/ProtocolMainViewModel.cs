using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Order), nameof(Order))]
public partial class ProtocolMainViewModel(ProtocolService protocolService, ReportService reportService, 
    ILogger<ProtocolMainViewModel> logger) : BaseViewModel(logger)
{
    [ObservableProperty]
    Order? order;

    [ObservableProperty]
    ObservableCollection<Protocol> protocols = new();

    [ObservableProperty]
    bool isRefreshing;

    [ObservableProperty]
    bool isEmptyList = true;

    [ObservableProperty]
    string search = string.Empty;

    [ObservableProperty]
    string filter = string.Empty;

    [RelayCommand]
    async Task GetProtocolsAsync() =>
        await DoBusyCommandAsync(async () =>
        {
            try
            {
                if (Order == null || Protocols.Any())
                    return;
                IsRefreshing = true;
                var protocols = await protocolService.GetProtocolsAsync(Order.Id);
                Protocols = protocols.ToObservableCollection();
            }
            finally
            {
                IsRefreshing = false;
            }
        },
        AppResources.GetProtocolsError);

    [RelayCommand]
    async Task AddProtocolAsync()
    {
        if (Protocols.Any())
            await CopyProtocolAsync(Protocols[0]);
        else
            await CreateProtocolAsync();
    }

    [RelayCommand]
    async Task CreateProtocolAsync()
    {
        Protocol? newProtocol = null;
        await DoBusyCommandAsync(async () =>
        {
            if (Order == null)
                return;
            newProtocol = await protocolService.CreateAsync(Order);
            Protocols.Insert(0, newProtocol);
        },
        AppResources.AddProtocolError);

        if (newProtocol != null)
            await GoToDetailsAsync(newProtocol);
    }

    [RelayCommand]
    async Task CopyProtocolAsync(Protocol protocol)
    {
        Protocol? newProtocol = null;
        await DoBusyCommandAsync(async () =>
        {
            newProtocol = await protocolService.CopyAsync(protocol);
            Protocols.Insert(0, newProtocol);
        },
        AppResources.CopyProtocolError);

        if (newProtocol != null)
            await GoToDetailsAsync(newProtocol);
    }

    [RelayCommand]
    async Task DeleteProtocolAsync(Protocol protocol) =>
        await DoBusyCommandAsync(async () =>
        {
            var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteProtocol, AppResources.Cancel, AppResources.Delete);

            if (string.Equals(action, AppResources.Cancel))
                return;

            await protocolService.DeleteAsync(protocol);
            Protocols.Remove(protocol);
        },
        protocol,
        AppResources.DeleteProtocolError);

    [RelayCommand]
    async Task CreateReportAsync(Protocol protocol) =>
        await DoBusyCommandAsync(async () =>
        {
            if (Order == null)
                return;
            await reportService.CreateSingleReportAsync(Order, protocol);
        },
        protocol,
        AppResources.CreateReportError);

    [RelayCommand]
    async Task GoToDetailsAsync(Protocol protocol) =>
        await DoBusyCommandAsync(async () =>
        {
            await Shell.Current.GoToAsync(nameof(ProtocolPage), true,
                new Dictionary<string, object> { { nameof(ProtocolViewModel.EditObject), protocol } });
            // await Shell.Current.GoToAsync($"//{nameof(ProtocolPage)}", true, new Dictionary<string, object> { { nameof(Protocol), protocol } });  //  modal form mode
        },
        protocol,
        AppResources.EditProtocolError);

    [RelayCommand]
    void FilterProtocols()
    {
        var searchValue = Search.Trim().ToLowerInvariant();
        var isOrderLocation = Order != null && Order.Location.ToLowerInvariant().Contains(searchValue);
        var isOrderAddress = Order != null && Order.Address.ToLowerInvariant().Contains(searchValue);
        var isOrderFireEscapeObject = Order != null && Order.FireEscapeObject.ToLowerInvariant().Contains(searchValue);

        Filter = $"(Contains([Location], '{searchValue}') or (IsNullOrEmpty([Location]) and {isOrderLocation}))" +
            $" or (Contains([Address], '{searchValue}') or (IsNullOrEmpty([Address]) and {isOrderAddress}))" +
            $" or (Contains([FireEscapeObject], '{searchValue}') or (IsNullOrEmpty([FireEscapeObject]) and {isOrderFireEscapeObject}))";
    }
}
