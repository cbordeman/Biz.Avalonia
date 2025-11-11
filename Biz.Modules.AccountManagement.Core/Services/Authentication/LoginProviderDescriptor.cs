namespace Biz.Modules.AccountManagement.Core.Services.Authentication;

public record LoginProviderDescriptor(
    string Name,
    string GeomertyResourceKey,
    Type ProviderType);
