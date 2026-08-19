namespace ServiceClients;

public interface IAuthDataStore
{
    TokenAndProvider? GetTokenAndProvider();
    AuthData? Data { get; set; }
    void RestoreAuthData();
    Task SaveAuthDataAsync();
    void RemoveAuthData();
}