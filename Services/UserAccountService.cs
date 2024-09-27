using Microsoft.Extensions.Options;
using Simusr2.Maui.DeviceIdentifier;
using System.Text.Json;

namespace FireEscape.Services;

public class UserAccountService(IFileHostingRepository fileHostingRepository, ILogger<UserAccountService> logger, IOptions<ApplicationSettings> applicationSettings) : IUserAccountService
{
    const string USER_ACCOUNT = "UserAccount";
    const string NEW_USER_NAME = "New User";
    const string USER_ACCOUNT_ID = "UserAccountId";
    const string CHECK_COUNTER = "CheckCounter";
    static string currentUserAccountId = string.Empty;
    static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

    readonly ApplicationSettings applicationSettings = applicationSettings.Value;
    readonly object syncObject = new();

    public async Task<UserAccount?> GetCurrentUserAccountAsync(bool download = false)
    {
        UserAccount? userAccount = null;

        if (!download)
        {
            userAccount = GetLocalUserAccount();
            if (userAccount == null)
            {
                GetUserAccountAsync().SafeFireAndForget(ex => logger.LogError(ex, ex.Message));
                return GetNewUserAccount();
            }
            CheckRemoteUserAccount(userAccount);
            return userAccount;
        }

        if (!await AppUtils.IsNetworkAccessAsync())
            return null;

        userAccount = await GetUserAccountAsync();
        return userAccount;
    }

    public async Task SaveAsync(UserAccount userAccount)
    {
        if (string.IsNullOrWhiteSpace(userAccount.Id) || !await AppUtils.IsNetworkAccessAsync())
            return;
        await UploadUserAccountAsync(userAccount);
        SetLocalUserAccount(userAccount);
    }

    public async Task DeleteAsync(UserAccount userAccount)
    {
        if (string.IsNullOrWhiteSpace(userAccount.Id) || !await AppUtils.IsNetworkAccessAsync())
            return;
        await fileHostingRepository.DeleteJsonAsync(userAccount.Id!, applicationSettings.UserAccountsFolderName);
        if (IsCurrentUserAccount(userAccount))
            Preferences.Default.Remove(USER_ACCOUNT);
    }

    public async IAsyncEnumerable<UserAccount> GetUserAccountsAsync()
    {
        if (!await AppUtils.IsNetworkAccessAsync())
            yield break;
        await foreach (var file in fileHostingRepository.ListFolderAsync(applicationSettings.UserAccountsFolderName))
        {
            var userAccountId = Path.GetFileNameWithoutExtension(file);
            var userAccount = await DownloadUserAccountAsync(userAccountId);
            if (userAccount != null)
                yield return userAccount;
        }
    }

    public void UpdateExpirationCount()
    {
        var userAccount = GetLocalUserAccount();
        if (userAccount?.ExpirationCount > 0)
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

    public async Task CheckCurrentUserAsync()
    {
        var userAccount = await GetCurrentUserAccountAsync();
        if (!IsValidUserAccount(userAccount))
            throw new Exception(string.Format(AppResources.UnregisteredApplicationMessage, CurrentUserAccountId));
    }

    public static bool IsValidUserAccount(UserAccount? userAccount) => userAccount != null && userAccount.IsValidUserAccount;

    public bool IsCurrentUserAccount(UserAccount? userAccount) => userAccount != null && userAccount.Id == CurrentUserAccountId;

    Task UploadUserAccountAsync(UserAccount userAccount) =>
        fileHostingRepository.UploadJsonAsync(userAccount.Id,
            JsonSerializer.Serialize(userAccount), applicationSettings.UserAccountsFolderName);

    async Task<UserAccount?> GetUserAccountAsync()
    {
        try
        {
            await semaphoreSlim.WaitAsync();

            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var userAccount = await DownloadUserAccountAsync(CurrentUserAccountId);
                if (userAccount == null)
                {
                    userAccount = GetNewUserAccount();
                    await UploadUserAccountAsync(userAccount);// upload a new user
                }
                else
                {
                    var expirationCount = 0;
                    var localUserAccount = GetLocalUserAccount();
                    if (localUserAccount != null)
                        expirationCount = localUserAccount.ExpirationCount; // save localUserAccount ExpirationCount

                    if (userAccount.ExpirationCount >= 0)
                    {
                        if (expirationCount < 0) expirationCount = 0; // reset unlimited count
                        expirationCount += userAccount.ExpirationCount; //increase localUserAccount ExpirationCount from remote
                        userAccount.ExpirationCount = 0; // clear remote ExpirationCount
                        await UploadUserAccountAsync(userAccount);
                    }
                    else
                        expirationCount = -1;  // set unlimited count

                    userAccount.ExpirationCount = expirationCount; // restore localUserAccount ExpirationCount
                    SetLocalUserAccount(userAccount);
                }
                return userAccount;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
        }
        finally
        {
            semaphoreSlim.Release();
        }
        return null;
    }

    async Task<UserAccount?> DownloadUserAccountAsync(string userAccountId)
    {
        var json = await fileHostingRepository.DownloadJsonAsync(userAccountId, applicationSettings.UserAccountsFolderName);
        return AppUtils.TryDeserialize<UserAccount>(json, out var userAccount) ? userAccount : null;
    }

    UserAccount? GetLocalUserAccount()
    {
        lock (syncObject)
        {
            var json = Preferences.Default.Get(USER_ACCOUNT, string.Empty);
            return AppUtils.TryDeserialize<UserAccount>(json, out var userAccount) ? userAccount : null;
        }
    }

    void SetLocalUserAccount(UserAccount userAccount)
    {
        if (IsCurrentUserAccount(userAccount))
        {
            lock (syncObject)
            {
                Preferences.Default.Set(USER_ACCOUNT, JsonSerializer.Serialize(userAccount));
                Preferences.Default.Set(CHECK_COUNTER, applicationSettings.CheckUserAccountCounter);
            }
        }
    }

    void CheckRemoteUserAccount(UserAccount userAccount)
    {
        if (userAccount.ExpirationCount > 0)
            return;

        var checkCounter = Preferences.Default.Get(CHECK_COUNTER, 0);
        if (checkCounter == 0 || !IsValidUserAccount(userAccount) || string.Equals(userAccount.Name, NEW_USER_NAME))
            GetUserAccountAsync().SafeFireAndForget(ex => logger.LogError(ex, ex.Message));
        if (checkCounter > 0)
            Preferences.Default.Set(CHECK_COUNTER, --checkCounter);
    }

    UserAccount GetNewUserAccount() => new UserAccount
    {
        Id = CurrentUserAccountId,
        Name = NEW_USER_NAME,
        Roles = [UserAccount.UserRole],
        Company = string.Empty,
        Signature = string.Empty,
        ExpirationCount = -1,
        ExpirationDate = DateTime.Now.AddYears(100)
    };
}