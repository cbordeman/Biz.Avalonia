using System;
using System.Threading.Tasks;
using Biz.Shared.Services;
using Security;
using Foundation;

namespace Biz.Shell.iOS.Services;

// ReSharper disable once InconsistentNaming
public class iOsSafeStorage : ISafeStorage
{
    const string ServiceId = "com.biz.app";

    public Task<string?> GetAsync(string key)
    {
        var query = new SecRecord(SecKind.GenericPassword)
        {
            Service = ServiceId,
            Account = key
        };

        var match = SecKeyChain.QueryAsRecord(query, out SecStatusCode resultCode);
        if (resultCode == SecStatusCode.Success && match != null)
        {
            var result = NSString.FromData(match.ValueData!, 
                NSStringEncoding.UTF8);
            return Task.FromResult<string?>(result);
        }

        return Task.FromResult<string?>(null);
    }

    public Task SetAsync(string key, string value)
    {
        Remove(key);

        var data = NSData.FromString(value, NSStringEncoding.UTF8);

        var record = new SecRecord(SecKind.GenericPassword)
        {
            Service = ServiceId,
            Account = key,
            ValueData = data,
            Accessible = SecAccessible.WhenUnlocked
        };

        var status = SecKeyChain.Add(record);
        if (status != SecStatusCode.Success)
            throw new Exception($"Could not add value to Keychain: {status}");

        return Task.CompletedTask;
    }

    public bool Remove(string key)
    {
        var record = new SecRecord(SecKind.GenericPassword)
        {
            Service = ServiceId,
            Account = key
        };
        var status = SecKeyChain.Remove(record);
        return status == SecStatusCode.Success;
    }

    public void RemoveAll()
    {
        var query = new SecRecord(SecKind.GenericPassword)
        {
            Service = ServiceId
        };
        SecKeyChain.Remove(query);
    }
}