using CommunityToolkit.Maui.Core.Extensions;
using FireEscape.Resources.Languages;
using System.Collections.ObjectModel;
using Protocol = FireEscape.Models.Protocol;

namespace FireEscape.ViewModels
{
    [QueryProperty(nameof(Order), nameof(Order))]
    public partial class ProtocolMainViewModel(ProtocolService protocolService, UserAccountService userAccountService) : BaseViewModel
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
            await DoCommandAsync(async () =>
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
            Protocol? newProtocol = null;
            await DoCommandAsync(async () =>
            {
                if (Order == null)
                    return;
                newProtocol = await protocolService.CreateProtocolAsync(Order);
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
            await DoCommandAsync(async () =>
            {
                newProtocol = await protocolService.CopyProtocolAsync(protocol);
                Protocols.Insert(0, newProtocol);
            },
            AppResources.CopyProtocolError);

            if (newProtocol != null)
                await GoToDetailsAsync(newProtocol);
        }

        [RelayCommand]
        async Task DeleteProtocolAsync(Protocol protocol) =>
            await DoCommandAsync(async () =>
            {
                var action = await Shell.Current.DisplayActionSheet(AppResources.DeleteProtocol, AppResources.Cancel, AppResources.Delete);

                if (string.Equals(action, AppResources.Cancel))
                    return;

                await protocolService.DeleteProtocol(protocol);
                Protocols.Remove(protocol);
            },
            protocol,
            AppResources.DeleteProtocolError);

        [RelayCommand]
        async Task CreateReportAsync(Protocol protocol) =>
            await DoCommandAsync(async () =>
            {
                var userAccount = await userAccountService.GetCurrentUserAccount();
                if (userAccount == null || Order == null)
                    return;
                if (UserAccountService.IsValidUserAccount(userAccount))
                {
                    userAccountService.UpdateExpirationCount(userAccount!);
                    await protocolService.CreateReportAsync(Order, protocol, userAccount!);
                }
                else
                {
                    await Shell.Current.DisplayAlert("",
                        string.Format(AppResources.UnregisteredApplicationMessage,
                        userAccountService.CurrentUserAccountId), AppResources.OK);
                }
            },
            protocol,
            AppResources.CreateReportError);

        [RelayCommand]
        async Task GoToDetailsAsync(Protocol protocol) =>
            await DoCommandAsync(async () =>
            {
                await Shell.Current.GoToAsync(nameof(ProtocolPage), true, new Dictionary<string, object> { { nameof(Protocol), protocol } });
                // await Shell.Current.GoToAsync($"//{nameof(ProtocolPage)}", true, new Dictionary<string, object> { { nameof(Protocol), protocol } });  //  modal form mode
            },
            protocol,
            AppResources.EditProtocolError);

        [RelayCommand]
        void FilterProtocols()
        {
            Filter = $"Contains([FullAddress], '{Search}') or Contains([FireEscapeObject], '{Search}')";
        }
    }
}
