using System.Threading.Tasks;
using Windows.Security.Credentials;
using Biz.Core.Services;

namespace Biz.Shell.Windows.Services;

public class WindowsSecureStorage : ISecureStorage
{
    private readonly PasswordVault vault = new();

    public Task<string?> GetAsync(string key)
    {
        try
        {
            var credential = vault.Retrieve("AvaloniaApp", key);
            credential.RetrievePassword();
            return Task.FromResult<string?>(credential.Password);
        }
        catch
        {
            return Task.FromResult<string?>(null);
        }
    }

    public Task SetAsync(string key, string value)
    {
        try
        {
            Remove(key);
        }
        catch { /* ignore if not found */ }

        vault.Add(new PasswordCredential("AvaloniaApp", key, value));
        return Task.CompletedTask;
    }

    public bool Remove(string key)
    {
        try
        {
            var credential = vault.Retrieve("AvaloniaApp", key);
            vault.Remove(credential);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public void RemoveAll()
    {
        var allCredentials = vault.FindAllByResource("AvaloniaApp");
        foreach (var c in allCredentials)
        {
            vault.Remove(c);
        }
    }
}