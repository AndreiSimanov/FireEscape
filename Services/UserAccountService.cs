using Microsoft.Extensions.Options;
using Simusr2.Maui.DeviceIdentifier;
using System.Text.Json;

namespace FireEscape.Services
{
    public class UserAccountService
    {
        const string USER_ACCOUNT = "UserAccount";
        const string NEW_USER_NAME = "New User";
        private const string USER_ACCOUNT_ID = "UserAccountId";
        private const string CHECK_COUNTER = "CheckCounter";
        private static string currentUserAccountId = string.Empty;

        readonly ApplicationSettings applicationSettings;
        readonly IFileHostingRepository fileHostingRepository;

        public UserAccountService(IFileHostingRepository fileHostingRepository, IOptions<ApplicationSettings> applicationSettings)
        {
            this.fileHostingRepository = fileHostingRepository;
            this.applicationSettings = applicationSettings.Value;
        }

        public async Task<UserAccount?> GetCurrentUserAccount(bool download = false)
        {
/*
#if DEBUG
            return new UserAccount { IsAdmin = true, 
                Name = "Debug User",  Company = "Debug Company",  
                Signature = "Debug User", Id = "debugUser", ExpirationCount = -1, ExpirationDate = DateTime.MaxValue};
#endif
*/
            UserAccount? userAccount = null;

            if (!download) 
            {
                userAccount = GetLocalUserAccount();
                if (IsValidUserAccount(userAccount))
                {
                    await CheckRemoteUserAccount(userAccount!);
                    return userAccount;
                }
            }

            if (!await AppUtils.IsNetworkAccess())
                return userAccount;

            userAccount = await DownloadUserAccountAsync(CurrentUserAccountId);

            if (userAccount == null)
                userAccount = await CreateNewUser(CurrentUserAccountId);
            else
            {
                if (userAccount.ExpirationCount > 0) // clear remote ExpirationCount
                {
                    var expirationCount = userAccount.ExpirationCount;
                    userAccount.ExpirationCount = 0;
                    await UploadUserAccountAsync(userAccount);
                    userAccount.ExpirationCount = expirationCount;
                }
                SetLocalUserAccount(userAccount);
            }    
            return userAccount;
        }

        public async Task SaveUserAccountAsync(UserAccount userAccount)
        {
            if (string.IsNullOrWhiteSpace(userAccount.Id) || !await AppUtils.IsNetworkAccess())
                return;
            await UploadUserAccountAsync(userAccount);
            SetLocalUserAccount(userAccount);
        }

        public async Task DeleteUserAccount(UserAccount userAccount)
        {
            if (string.IsNullOrWhiteSpace(userAccount.Id) || !await AppUtils.IsNetworkAccess())
                return;
            await fileHostingRepository.DeleteJsonAsync(userAccount.Id!, applicationSettings.UserAccountsFolderName);
            if (IsCurrentUserAccount(userAccount))
                Preferences.Default.Remove(USER_ACCOUNT);
        }

        public async IAsyncEnumerable<UserAccount> GetUserAccountsAsync()
        {
            if (!await AppUtils.IsNetworkAccess())
                yield break;
            await foreach (var file in fileHostingRepository.ListFolderAsync(applicationSettings.UserAccountsFolderName))
            {
                var userAccountId = Path.GetFileNameWithoutExtension(file);
                var userAccount = await DownloadUserAccountAsync(userAccountId);
                if (userAccount != null)
                    yield return userAccount;
            }
        }

        public void UpdateExpirationCount(UserAccount userAccount)
        {
            if (IsCurrentUserAccount(userAccount) && userAccount.ExpirationCount > 0)
            {
                userAccount.ExpirationCount--;
                SetLocalUserAccount(userAccount);
            }
        }

        public string CurrentUserAccountId
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(currentUserAccountId))
                    return currentUserAccountId;

                var userAccountId = Identifier.Get();
                if (string.IsNullOrWhiteSpace(userAccountId))
                {
                    userAccountId = Preferences.Get(USER_ACCOUNT_ID, string.Empty);
                    if (string.IsNullOrWhiteSpace(userAccountId))
                    {
                        userAccountId = Guid.NewGuid().ToString();
                        Preferences.Set(USER_ACCOUNT_ID, userAccountId);
                    }
                }
                return userAccountId;
            }
        }

        public bool IsValidUserAccount(UserAccount? userAccount)
        {
            return userAccount != null && userAccount.IsValidUserAccount;
        }

        public bool IsCurrentUserAccount(UserAccount? userAccount)
        {
            return userAccount != null && userAccount.Id == CurrentUserAccountId;
        }

        private async Task<UserAccount> CreateNewUser(string userAccountId)
        {
            var userAccount = new UserAccount()
            {
                Id = userAccountId,
                Name = NEW_USER_NAME,
                Roles = new List<string> { UserAccount.UserRole },
                ExpirationDate = DateTime.Now.AddDays(applicationSettings.NewUserAccountExpirationDays),
                ExpirationCount = 0
            };

            await UploadUserAccountAsync(userAccount);

            if (IsCurrentUserAccount(userAccount))
            {
                userAccount.ExpirationCount = applicationSettings.NewUserAccountExpirationCount;
                SetLocalUserAccount(userAccount);
            }
            return userAccount;
        }

        private async Task UploadUserAccountAsync(UserAccount userAccount)
        {
            await fileHostingRepository.UploadJsonAsync(userAccount.Id, JsonSerializer.Serialize(userAccount), applicationSettings.UserAccountsFolderName);
        }

        private async Task<UserAccount?> DownloadUserAccountAsync(string userAccountId)
        {
            UserAccount? userAccount = null;
            var json = await fileHostingRepository.DownloadJsonAsync(userAccountId, applicationSettings.UserAccountsFolderName);
            if (!string.IsNullOrWhiteSpace(json))
                userAccount = JsonSerializer.Deserialize<UserAccount>(json);
            return userAccount;
        }

        private void SetLocalUserAccount(UserAccount userAccount)
        {
            if (IsCurrentUserAccount(userAccount))
            {
                Preferences.Default.Set(USER_ACCOUNT, JsonSerializer.Serialize(userAccount));
                Preferences.Default.Set(CHECK_COUNTER, applicationSettings.CheckUserAccountCounter);
            }
        }

        private UserAccount? GetLocalUserAccount()
        {
            var json = Preferences.Default.Get(USER_ACCOUNT, string.Empty);
            return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<UserAccount>(json);
        }

        private async Task CheckRemoteUserAccount(UserAccount userAccount)
        {
            if (userAccount.ExpirationCount > 0)
                return;

            var checkCounter = Preferences.Default.Get(CHECK_COUNTER, 0);

            if (checkCounter == 0)
            {
                Preferences.Default.Remove(USER_ACCOUNT); // perform to download UserAccount
                return;
            }
            
            if (checkCounter < applicationSettings.CheckUserAccountCounter / 2  // try to upload & update UserAccount 
                && Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    var remoteUserAccount = await DownloadUserAccountAsync(userAccount.Id);
                    if (remoteUserAccount == null)
                        Preferences.Default.Remove(USER_ACCOUNT);
                    else
                        SetLocalUserAccount(remoteUserAccount);
                    return;
                }
                catch
                {
                }
            }
            Preferences.Default.Set(CHECK_COUNTER, --checkCounter);
        }
    }
}