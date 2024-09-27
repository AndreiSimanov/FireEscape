using CommunityToolkit.Maui.Core.Extensions;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels;

[QueryProperty(nameof(Order), nameof(Order))]
public partial class ProtocolMainViewModel(IProtocolService protocolService, IReportService reportService,
    ILogger<ProtocolMainViewModel> logger) : BaseViewModel(logger)
{
    [ObservableProperty]
    Order? order;

    [ObservableProperty]
    ObservableCollection<Protocol> protocols = [];

    [ObservableProperty]
    object? selectedItem = null;

    [ObservableProperty]
    bool isRefreshing;

    [ObservableProperty]
    bool isEmptyList = true;

    [ObservableProperty]
    string search = string.Empty;

    [ObservableProperty]
    string filter = string.Empty;

    [RelayCommand]
    Task GetProtocolsAsync() =>
        DoBusyCommandAsync(async () =>
        {
            try
            {
                if (Order == null || Protocols.Any())
                    return;
                IsRefreshing = true;
                SelectedItem = null;
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
    Task AddProtocolAsync() => Protocols.Count == 0 ? CreateProtocolAsync() : CopyProtocolAsync(Protocols[0]);

    [RelayCommand]
    Task CopyProtocolWithStairsAsync(Protocol protocol) =>
        CreateProtocolAsync(protocolService.CopyWithStairsAsync(protocol), AppResources.CopyProtocolError);

    [RelayCommand]
    Task DeleteProtocolAsync(Protocol protocol) =>
        DoBusyCommandAsync(async () =>
        {
            SelectedItem = protocol;
            var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteProtocol, AppResources.Cancel, AppResources.Delete);

            if (string.Equals(action, AppResources.Delete))
            {
                await protocolService.DeleteAsync(protocol);
                Protocols.Remove(protocol);
                SelectedItem = null;
            }
        },
        protocol,
        AppResources.DeleteProtocolError);

    [RelayCommand]
    Task CreateReportAsync(Protocol protocol) =>
        DoBusyCommandAsync(() =>
        {
            SelectedItem = protocol;
            if (Order == null)
                return Task.CompletedTask;
            return reportService.CreateSingleReportAsync(Order, protocol, Protocols.Count > 1);
        },
        protocol,
        AppResources.CreateReportError);

    [RelayCommand]
    Task GoToDetailsAsync(Protocol protocol) =>
        DoBusyCommandAsync(() => GoToAsync(protocol),
        protocol,
        AppResources.EditProtocolError);

    [RelayCommand]
    void FilterItems() =>
        DoCommand(() =>
        {
            var searchValue = Search.Trim().ToLowerInvariant();
            var isOrderLocation = Order != null && Order.Location.ToLowerInvariant().Contains(searchValue);
            var isOrderAddress = Order != null && Order.Address.ToLowerInvariant().Contains(searchValue);
            var isOrderFireEscapeObject = Order != null && Order.FireEscapeObject.ToLowerInvariant().Contains(searchValue);

            Filter = $"(Contains([Location], '{searchValue}') or (IsNullOrEmpty([Location]) and {isOrderLocation}))" +
                $" or (Contains([Address], '{searchValue}') or (IsNullOrEmpty([Address]) and {isOrderAddress}))" +
                $" or (Contains([FireEscapeObject], '{searchValue}') or (IsNullOrEmpty([FireEscapeObject]) and {isOrderFireEscapeObject}))";
        },
        AppResources.GetProtocolsError);

    Task CreateProtocolAsync()
    {
        if (Order == null)
            return Task.CompletedTask;
        return CreateProtocolAsync(protocolService.CreateAsync(Order), AppResources.AddProtocolError);
    }

    Task CopyProtocolAsync(Protocol protocol) =>
        CreateProtocolAsync(protocolService.CopyAsync(protocol), AppResources.CopyProtocolError);

    Task CreateProtocolAsync(Task<Protocol> task, string exceptionCaption) =>
        DoBusyCommandAsync(async () =>
        {
            var newProtocol = await task;
            Protocols.Insert(0, newProtocol);
            SelectedItem = newProtocol;
            if (newProtocol != null)
                await GoToAsync(newProtocol);
        },
        exceptionCaption);

    Task GoToAsync(Protocol protocol)
    {
        SelectedItem = protocol;
        return Shell.Current.GoToAsync(nameof(ProtocolPage), true,
            new Dictionary<string, object> { { nameof(ProtocolViewModel.EditObject), protocol } });
        // return Shell.Current.GoToAsync($"//{nameof(ProtocolPage)}", true, new Dictionary<string, object> { { nameof(Protocol), protocol } });  //  modal form mode
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.PropertyName == nameof(Order))
        {
            Protocols = [];
            Search = string.Empty;
            SelectedItem = null;
        }
    }
}
