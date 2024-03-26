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
        readonly IFileHostingRepository fileHostingRepository;

        public UserAccountService(IFileHostingRepository fileHostingRepository, IOptions<ApplicationSettings> applicationSettings)
        {
            this.fileHostingRepository = fileHostingRepository;
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

        public async IAsyncEnumerable<UserAccount> GetUserAccountsAsync()
        {
            await foreach (var file in fileHostingRepository.ListFolderAsync(applicationSettings.UserAccountsFolderName))
            {
                var keyEnumeration = Path.GetFileNameWithoutExtension(file);
                var userAccountJson = await fileHostingRepository.DownloadJsonAsync(keyEnumeration, applicationSettings.UserAccountsFolderName);
                UserAccount? userAccount = null;
                try
                {
                    userAccount = JsonSerializer.Deserialize<UserAccount>(userAccountJson);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"GetUserAccountsAsync: {ex.Message}");
                }
                if (userAccount != null)
                    yield return userAccount;
            }
        }

        public async Task SaveUserAccountAsync(UserAccount userAccount)
        {
            await fileHostingRepository.UploadJsonAsync(userAccount.Id!, JsonSerializer.Serialize(userAccount), applicationSettings.UserAccountsFolderName);
        }

        public async Task DeleteUserAccount(UserAccount userAccount)
        {
            await fileHostingRepository.DeleteJsonAsync(userAccount.Id!, applicationSettings.UserAccountsFolderName);
        }

        public async Task<UserAccount?> DownloadUserAccountAsync()
        {
            if (string.IsNullOrWhiteSpace(AppSettingsExtension.UserAccountId))
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
                json = await fileHostingRepository.DownloadJsonAsync(AppSettingsExtension.UserAccountId, applicationSettings.UserAccountsFolderName);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"DownloadJsonAsync: {ex.Message}");
            }

            if (string.IsNullOrWhiteSpace(json))
            {
                userAccount = new UserAccount()
                {
                    Id = AppSettingsExtension.UserAccountId,
                    Roles = new List<string> { UserAccount.UserRole },
                    ExpirationDate = DateTime.Now
                };

                json = JsonSerializer.Serialize(userAccount);

                try
                {

                    await fileHostingRepository.UploadJsonAsync(AppSettingsExtension.UserAccountId, json, applicationSettings.UserAccountsFolderName);
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
                && string.Equals(userAccount.Id, AppSettingsExtension.UserAccountId);
        }
    }
}