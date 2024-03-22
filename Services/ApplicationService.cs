using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace FireEscape.Services
{
    public class ApplicationService
    {
        const string USER_ACCOUNT = "UserAccount";
        readonly ApplicationSettings applicationSettings;
        readonly DropboxRepository dropboxRepository;
        

        public ApplicationService(DropboxRepository dropboxRepository, IOptions<ApplicationSettings> applicationSettings)
        {
            this.dropboxRepository = dropboxRepository;
            this.applicationSettings = applicationSettings.Value;
        }

        public async Task<bool> CheckApplicationExpiration()
        {
            return IsValidUserAccount(await GetUserAccount());
        }

        private async Task<UserAccount?> GetUserAccount()
        {
            var json = Preferences.Default.Get(USER_ACCOUNT, string.Empty);
            var userAccount = string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<UserAccount>(json);

            if (IsValidUserAccount(userAccount))
                return userAccount;

            userAccount = await DownloadUserAccountAsync();

            return userAccount;
        }

        async private Task<UserAccount?> DownloadUserAccountAsync ()
        {
            if (string.IsNullOrWhiteSpace( AppSettingsExtension.DeviceIdentifier))
                return null;
           
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                await Shell.Current.DisplayAlert(AppResources.NoConnectivity, AppResources.CheckInternetMessage, AppResources.OK);
                return null;
            }

            UserAccount? userAccount;
            string? json = string.Empty;

            try
            {
                json = await dropboxRepository.DownloadJsonAsync(AppSettingsExtension.DeviceIdentifier, applicationSettings.UserAccountsFolderName);
            }
            catch (Exception ex) 
            {
                Debug.WriteLine($"DownloadJsonAsync: {ex.Message}");
            }


            if (string.IsNullOrWhiteSpace(json))
            {
                userAccount = new UserAccount() { Id = AppSettingsExtension.DeviceIdentifier, ExpirationDate = DateTime.Now };
                json = JsonSerializer.Serialize(userAccount);

                try
                {

                    await dropboxRepository.UploadJsonAsync(AppSettingsExtension.DeviceIdentifier, json, applicationSettings.UserAccountsFolderName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"UploadJsonAsync: {ex.Message}");
                }

            }
            else
                userAccount = JsonSerializer.Deserialize<UserAccount>(json);

            Preferences.Default.Set(USER_ACCOUNT, json);
            return userAccount;
        }

        private bool IsValidUserAccount(UserAccount? userAccount)
        {
            return
                userAccount != null
                && !string.IsNullOrWhiteSpace(userAccount.Id)
                && !string.IsNullOrWhiteSpace(userAccount.Name)
                && userAccount.ExpirationDate != null
                && userAccount.ExpirationDate > DateTime.Now
                && string.Equals(userAccount.Id, AppSettingsExtension.DeviceIdentifier);
        }
    }

    public class UserAccount
    { 
        public string? Id { get; set; }
        public string? Name { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}