using System.Threading.Tasks;
using Biz.Shared.Services;
using CloudNimble.BlazorEssentials.IndexedDb;
using JetBrains.Annotations;
using Microsoft.JSInterop;
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Biz.Shell.Browser.Services;

[UsedImplicitly]
public class SecureStorageDb : IndexedDbDatabase
{
    // Define a store for KeyValue pairs
    [ObjectStore(Name = "BizKeyValueStore")]
    public IndexedDbObjectStore? KeyValueStore { get; set; }

    public SecureStorageDb(IJSRuntime jsRuntime) : base(jsRuntime)
    {
        Name = "BizSecureStorage";
        Version = 1;
    }
}

public class WasmSecureStorage : ISafeStorage
{
    private readonly SecureStorageDb db;

    public WasmSecureStorage(SecureStorageDb db)
    {
        this.db = db;
    }

    public async Task InitializeAsync()
    {
        await db.OpenAsync();
    }

    public async Task<string?> GetAsync(string key)
    {
        var item = await db.KeyValueStore!.GetAsync<string, KeyValueItem>(key);
        return item?.Value;
    }

    public async Task SetAsync(string key, string value)
    {
        var item = new KeyValueItem { Key = key, Value = value };
        await db.KeyValueStore!.PutAsync(item);
    }

    public bool Remove(string key)
    {
        _ = db.KeyValueStore!.DeleteAsync(key);
        return true;
    }

    public void RemoveAll()
    {
        _ = db.KeyValueStore!.ClearStoreAsync();
    }
}

public class KeyValueItem
{
    public string? Key { get; init; }
    public string? Value { get; init; }
}