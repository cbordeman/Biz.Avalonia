using System.Threading.Tasks;
using Biz.Core.Services;

namespace Biz.Shell.Browser.Services;

public class BrowserSafeStorage : ISafeStorage
{
    public Task<string?> GetAsync(string key)
    {
        throw new System.NotImplementedException();
    }

    public Task SetAsync(string key, string value)
    {
        throw new System.NotImplementedException();
    }

    public bool Remove(string key)
    {
        throw new System.NotImplementedException();
    }

    public void RemoveAll()
    {
        throw new System.NotImplementedException();
    }
}