using Microsoft.Extensions.Options;
using Simusr2.Maui.DeviceIdentifier;
using System.Text.Json;

namespace FireEscape.Services;

public class UserAccountService(IFileHostingRepository fileHostingRepository, IOptions<ApplicationSettings> applicationSettings)
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
                _ = Task.Run(TryToGetUserAccountAsync);
                return GetNewUserAccount();
            }
            CheckRemoteUserAccount(userAccount);
            return userAccount;
        }

        if (!await AppUtils.IsNetworkAccessAsync())
            return null;

        userAccount = await TryToGetUserAccountAsync();
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

    public async IAsyncEnumerable<UserAccount> GetAsync()
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

    public static bool IsValidUserAccount(UserAccount? userAccount) => userAccount != null && userAccount.IsValidUserAccount;

    public bool IsCurrentUserAccount(UserAccount? userAccount) => userAccount != null && userAccount.Id == CurrentUserAccountId;

    async Task UploadUserAccountAsync(UserAccount userAccount) =>
        await fileHostingRepository.UploadJsonAsync(userAccount.Id,
            JsonSerializer.Serialize(userAccount), applicationSettings.UserAccountsFolderName);

    async Task<UserAccount?> TryToGetUserAccountAsync()
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
        catch { }
        finally
        {
            semaphoreSlim.Release();
        }
        return null;
    }

    async Task<UserAccount?> DownloadUserAccountAsync(string userAccountId)
    {
        UserAccount? userAccount = null;
        var json = await fileHostingRepository.DownloadJsonAsync(userAccountId, applicationSettings.UserAccountsFolderName);
        if (!string.IsNullOrWhiteSpace(json))
            userAccount = JsonSerializer.Deserialize<UserAccount>(json);
        return userAccount;
    }

    UserAccount? GetLocalUserAccount()
    {
        lock (syncObject)
        {
            var json = Preferences.Default.Get(USER_ACCOUNT, string.Empty);
            return string.IsNullOrWhiteSpace(json) ? null : JsonSerializer.Deserialize<UserAccount>(json);
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
        if (checkCounter == 0 || !IsValidUserAccount(userAccount) )
            _ = Task.Run(TryToGetUserAccountAsync);
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