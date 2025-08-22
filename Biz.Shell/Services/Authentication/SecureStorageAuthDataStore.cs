using System.Text.Json;
using Biz.Core.Services;
using ServiceClients;

namespace Biz.Shell.Services.Authentication;

internal class SecureStorageAuthDataStore : IAuthDataStore
{
    readonly ISafeStorage secureStorage;

    const string AuthDataKey = "auth_data"; // Key for SecureStorage

    public AuthData? Data { get; set; }

    public SecureStorageAuthDataStore(ISafeStorage secureStorage)
    {
        this.secureStorage = secureStorage;
    }
    
    public async Task RestoreAuthDataAsync()
    {
        try
        {
            var json = await secureStorage.GetAsync(AuthDataKey);
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    Data = JsonSerializer.Deserialize<AuthData>(json);
                    return;
                }
                catch
                {
                    Data = null;
                }
            }
            Data = null;
        }
        catch (Exception e)
        {
            
        }
    }

    public async Task<TokenAndProvider?> GetTokenAndProvider()
    {
        if (Data == null)
            await RestoreAuthDataAsync();
        if (Data == null)
            return null;

        //// If it's a Facebook token, validate it
        //if (Data.Provider == LoginProvider.Facebook)
        //    return await ValidateFacebookTokenAsync(authData.AccessToken);

        if (Data.ExpiresAt <= DateTimeOffset.UtcNow.AddMinutes(-10))
            return null;
        if (Data?.LoginProvider == null)
            throw new InvalidOperationException("Data.LoginProvider was null");
        return new TokenAndProvider(Data.AccessToken!, Data.LoginProvider ?? LoginProvider.Microsoft);
    }

    public async Task SaveAuthDataAsync()
    {
        var json = JsonSerializer.Serialize(Data);
        await secureStorage.SetAsync(AuthDataKey, json);
    }

    public void RemoveAuthData()
    {
        secureStorage.Remove(AuthDataKey);
        Data = new AuthData();
    }

}