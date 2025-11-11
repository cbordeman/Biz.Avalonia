using Biz.Modules.AccountManagement.Core.Services.Authentication;

namespace Biz.Modules.AccountManagement.Services.Authentication;

public class LoginProviderRegistry : ILoginProviderRegistry
{
    readonly Dictionary<LoginProvider, LoginProviderDescriptor> 
        loginProviders = new();
    
    public void RegisterLoginProvider<T>(
        LoginProvider providerEnum, string name, 
        string geometryResourceKey)
        where T : class, IClientLoginProvider
    {
        if (loginProviders.ContainsKey(providerEnum))
            throw new InvalidOperationException(
                $"Login provider for {providerEnum} is already registered.");
        loginProviders.Add(providerEnum, 
            new LoginProviderDescriptor(
                name, geometryResourceKey, typeof(T)));
    }

    public IReadOnlyDictionary<LoginProvider, LoginProviderDescriptor> 
        Descriptors => loginProviders.AsReadOnly();
}
