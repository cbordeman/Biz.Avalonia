using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Android.Runtime;
using Biz.Core.Services;
using Microsoft.Extensions.Logging;
using Xamarin.Google.Crypto.Tink;
using Xamarin.Google.Crypto.Tink.Aead;
using Xamarin.Google.Crypto.Tink.Integration.Android;

namespace Biz.Shell.Android.Services;

#pragma warning disable CS0618 // Type or member is obsolete

public class AndroidSafeStorage : ISafeStorage
{
    private readonly ILogger<AndroidSafeStorage> logger;
    private readonly Java.IO.File backingFile;
    private readonly Lock syncRoot = new();

    private readonly IAead aead;

    public AndroidSafeStorage(ILogger<AndroidSafeStorage> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        backingFile = new Java.IO.File(MainActivity.Context.FilesDir, "secure_store.json");

        try
        {
            // Register Tink AEAD
            AeadConfig.Register();

            var handle = new AndroidKeysetManager.Builder()
                .WithSharedPref(MainActivity.Context, "tink_keyset", "tink_prefs")!
                .WithKeyTemplate(AeadKeyTemplates.Aes256Gcm)!
                .WithMasterKeyUri("android-keystore://tink_master_key")!
                .Build()!
                .KeysetHandle;

            if (handle != null)
            {
                var tmp = handle.GetPrimitive(Java.Lang.Class.FromType(typeof(IAead)));
                if (tmp == null)
                    throw new Exception("Failed to get AEAD primitive.");

                aead = tmp.JavaCast<IAead>();
            }

            if (aead == null)
                throw new Exception("Failed to initialize AEAD.");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to initialize secure storage");
            throw;
        }
    }

    private Dictionary<string, string> LoadAll()
    {
        lock (syncRoot)
        {
            if (!backingFile.Exists()) return new Dictionary<string, string>();
            try
            {
                var bytes = File.ReadAllBytes(backingFile.AbsolutePath);
                var decrypted = aead.Decrypt(bytes, null);
                var json = Encoding.UTF8.GetString(decrypted!);
                return JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                       ?? new Dictionary<string, string>();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to decrypt storage file.");
                return new Dictionary<string, string>();
            }
        }
    }

    private void SaveAll(Dictionary<string, string> dict)
    {
        lock (syncRoot)
        {
            try
            {
                var json = JsonSerializer.Serialize(dict);
                var plainBytes = Encoding.UTF8.GetBytes(json);
                var cipherBytes = aead.Encrypt(plainBytes, null)!;

                File.WriteAllBytes(path: backingFile.AbsolutePath, cipherBytes);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to encrypt storage file.");
                throw;
            }
        }
    }

    public Task<string?> GetAsync(string key)
    {
        var dict = LoadAll();
        dict.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    public Task SetAsync(string key, string value)
    {
        var dict = LoadAll();
        dict[key] = value;
        SaveAll(dict);
        return Task.CompletedTask;
    }

    public bool Remove(string key)
    {
        var dict = LoadAll();
        if (dict.Remove(key)) { SaveAll(dict); return true; }
        return false;
    }

    public void RemoveAll()
    {
        SaveAll(new Dictionary<string, string>());
    }
}