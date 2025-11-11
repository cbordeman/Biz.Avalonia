using Biz.Models;

namespace Biz.Modules.AccountManagement.Core.Services.Authentication;

public interface ILoginProviderRegistry
{
    void RegisterLoginProvider<T>(
        LoginProvider providerEnum, string name,
        string geometryResourceKey)
        where T : class, IClientLoginProvider;
    IReadOnlyDictionary<LoginProvider, LoginProviderDescriptor> 
        Descriptors { get; }
}
