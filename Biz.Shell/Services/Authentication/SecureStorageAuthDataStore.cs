using System.Text.Json;
using Biz.Core;
using Microsoft.Maui.Storage;
using ServiceClients;

namespace Biz.Shell.Services.Authentication;

internal class SecureStorageAuthDataStore : IAuthDataStore
{
    const string AuthDataKey = "auth_data"; // Key for SecureStorage

    public AuthData? Data { get; set; }

    public async Task RestoreAuthDataAsync()
    {
        var json = await SecureStorage.GetAsync(AuthDataKey);
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
        await SecureStorage.SetAsync(AuthDataKey, json);
    }

    public void RemoveAuthData()
    {
        SecureStorage.Remove(AuthDataKey);
        Data = new AuthData();
    }

}