using Biz.Shell.ClientLoginProviders;

namespace Biz.Shell.Services.Authentication;

public record LoginProviderDescriptor(
    string Name,
    string GeomertyResourceKey,
    Type ProviderType);

public class LoginProviderRegistry
{
    readonly Dictionary<LoginProvider, LoginProviderDescriptor> 
        loginProviders = new();
    
    public void RegisterLoginProvider<T>(
        LoginProvider providerEnum, string name, 
        string geomertyResourceKey)
        where T : class, IClientLoginProvider
    {
        if (loginProviders.ContainsKey(providerEnum))
            throw new InvalidOperationException(
                $"Login provider for {providerEnum} is already registered.");
        loginProviders.Add(providerEnum, 
            new LoginProviderDescriptor(
                name, geomertyResourceKey, typeof(T)));
    }

    public ReadOnlyDictionary<LoginProvider, LoginProviderDescriptor> 
        Descriptors => loginProviders.AsReadOnly();
}
