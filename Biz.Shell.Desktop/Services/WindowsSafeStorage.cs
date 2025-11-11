using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Biz.Authentication;
using Biz.Shared.Services;

namespace Biz.Shell.Desktop.Services;

public class WindowsSafeStorage : ISafeStorage
{
    readonly string storageFilePath;
    readonly object lockObj = new();
    StorageData? cache;

    public WindowsSafeStorage()
    {
        var appDataPath = Environment.GetFolderPath(
            Environment.SpecialFolder.ApplicationData);
        var appFolder = Path.Combine(appDataPath, "MyAvaloniaApp");
        Directory.CreateDirectory(appFolder);
        storageFilePath = Path.Combine(appFolder, "securestorage.json");
    }

    public async Task<string?> GetAsync(string key)
    {
        await LoadCacheAsync();
        lock (lockObj)
            if (cache != null && cache.Data.TryGetValue(key, out var value))
                return value;
            else
                return null;
    }

    public async Task SetAsync(string key, string value)
    {
        await LoadCacheAsync();
        lock (lockObj)
        {
            if (cache == null)
                cache = new StorageData();
            cache.Data[key] = value;
        }
        await SaveCacheAsync();
    }

    public bool Remove(string key)
    {
        var removed = false;
        lock (lockObj)
        {
            if (cache != null)
            {
                removed = cache.Data.Remove(key);
                if (removed)
                    _ = SaveCacheAsync();
            }
        }
        return removed;
    }

    public void RemoveAll()
    {
        lock (lockObj)
            cache = new StorageData();
        _ = SaveCacheAsync();
    }

    async Task LoadCacheAsync()
    {
        if (cache != null)
            return;

        if (File.Exists(storageFilePath))
        {
            try
            {
                using var stream = File.OpenRead(storageFilePath);
                cache = await JsonSerializer.DeserializeAsync<StorageData>(stream) 
                        ?? new StorageData();
            }
            catch
            {
                cache = new StorageData();
            }
        }
        else
            cache = new StorageData();
    }

    async Task SaveCacheAsync()
    {
        try
        {
            using var stream = File.Create(storageFilePath);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            await JsonSerializer.SerializeAsync(stream, cache, options);
        }
        catch
        {
            // Optionally handle saving exceptions here
        }
    }

    class StorageData
    {
        public Dictionary<string, string> Data { get; set; } = new();
    }
}