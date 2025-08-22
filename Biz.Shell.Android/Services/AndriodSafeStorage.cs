using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Android.Content;
using AndroidX.Security.Crypto;
using Biz.Core.Services;
using Microsoft.Extensions.Logging;

namespace Biz.Shell.Android.Services;

public class AndroidSafeStorage : ISafeStorage
{
    readonly ILogger<AndroidSafeStorage> logger;
    readonly ISharedPreferences prefs;

    public AndroidSafeStorage(ILogger<AndroidSafeStorage> logger)
    {
        this.logger = logger;
        try
        {
            var masterKeyAlias = new MasterKey.Builder(MainActivity.Context)
                .SetKeyScheme(MasterKey.KeyScheme.Aes256Gcm!)
                .Build();

            Debug.Assert(EncryptedSharedPreferences.PrefKeyEncryptionScheme.Aes256Siv != null);
            Debug.Assert(EncryptedSharedPreferences.PrefValueEncryptionScheme.Aes256Gcm != null);

            prefs = EncryptedSharedPreferences.Create(
                MainActivity.Context,
                "biz_secure_prefs",
                masterKeyAlias,
                EncryptedSharedPreferences.PrefKeyEncryptionScheme.Aes256Siv,
                EncryptedSharedPreferences.PrefValueEncryptionScheme.Aes256Gcm);
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to construct {nameof(EncryptedSharedPreferences)}");
            throw;
        }
    }

    public Task<string?> GetAsync(string key)
    {
        return Task.FromResult(prefs.GetString(key, null));
    }

    public Task SetAsync(string key, string value)
    {
        var editor = prefs.Edit();
        Debug.Assert(editor != null);
        editor.PutString(key, value);
        editor.Apply();
        return Task.CompletedTask;
    }

    public bool Remove(string key)
    {
        if (prefs.Contains(key))
        {
            var editor = prefs.Edit();
            Debug.Assert(editor != null);
            editor.Remove(key);
            editor.Apply();
            return true;
        }
        return false;
    }

    public void RemoveAll()
    {
        var editor = prefs.Edit();
        Debug.Assert(editor != null);
        editor.Clear();
        editor.Apply();
    }
}
