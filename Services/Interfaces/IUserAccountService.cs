namespace FireEscape.Services.Interfaces;

public interface IUserAccountService
{
    string CurrentUserAccountId { get; }

    Task CheckCurrentUserAsync();
    Task DeleteAsync(UserAccount userAccount);
    Task<UserAccount?> GetCurrentUserAccountAsync(bool download = false);
    IAsyncEnumerable<UserAccount> GetUserAccountsAsync();
    bool IsCurrentUserAccount(UserAccount? userAccount);
    Task SaveAsync(UserAccount userAccount);
    void UpdateExpirationCount();
}