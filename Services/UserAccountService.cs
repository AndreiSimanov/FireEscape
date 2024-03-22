using FireEscape.Resources.Languages;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace FireEscape.Services
{
    public class UserAccountService
    {
        const string USER_ACCOUNT = "UserAccount";
        readonly ApplicationSettings applicationSettings;
        readonly DropboxRepository dropboxRepository;
        

        public UserAccountService(DropboxRepository dropboxRepository, IOptions<ApplicationSettings> applicationSettings)
        {
            this.dropboxRepository = dropboxRepository;
            this.applicationSettings = applicationSettings.Value;
        }

        public async Task<bool> CheckApplicationExpiration()
        {
            return IsValidUserAccount(await GetUserAccount());
        }

        public async Task<UserAccount?> GetUserAccount()
        {
            var json = Preferences.Default.Get(USER_ACCOUNT, string.Empty);
            var userAccount = string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<UserAccount>(json);

            if (IsValidUserAccount(userAccount))
                return userAccount;

            userAccount = await DownloadUserAccountAsync();

            return userAccount;
        }

        public async Task<UserAccount?> DownloadUserAccountAsync ()
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
                userAccount = new UserAccount() { 
                    Id = AppSettingsExtension.DeviceIdentifier, 
                    Roles = new List<string> { UserAccount.UserRole }, 
                    ExpirationDate = DateTime.Now };

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

        public bool IsValidUserAccount(UserAccount? userAccount)
        {
            return
                userAccount != null
                && !string.IsNullOrWhiteSpace(userAccount.Id)
                && !string.IsNullOrWhiteSpace(userAccount.Name)
                && userAccount.ExpirationDate != null
                && userAccount.ExpirationDate > DateTime.Now
                && string.Equals(userAccount.Id, AppSettingsExtension.DeviceIdentifier);
        }

        public async IAsyncEnumerable<UserAccount> GetUserAccountsAsync()
        {
            var listFolderResult = await dropboxRepository.ListFolderAsync(applicationSettings.UserAccountsFolderName);
            if (listFolderResult != null && listFolderResult.Entries.Any())
            {
                var keyEnumeration = listFolderResult.Entries.Select(file => Path.GetFileNameWithoutExtension(file.Name));
                await foreach (var item in dropboxRepository.DownloadJsonAsync(keyEnumeration, applicationSettings.UserAccountsFolderName))
                {
                    UserAccount? userAccount = null;
                    try
                    {
                        userAccount = JsonSerializer.Deserialize<UserAccount>(item);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"GetUserAccountsAsync: {ex.Message}");
                    }

                    if (userAccount != null)
                        yield return userAccount;
                }
            }
        }
    }
}