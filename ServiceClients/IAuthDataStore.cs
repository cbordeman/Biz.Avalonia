namespace ServiceClients;

public interface IAuthDataStore
{
    Task<TokenAndProvider?> GetTokenAndProvider();
    AuthData? Data { get; set; }
    Task RestoreAuthDataAsync();
    Task SaveAuthDataAsync();
    void RemoveAuthData();
}