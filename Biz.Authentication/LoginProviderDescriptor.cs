namespace Biz.Authentication;

public record LoginProviderDescriptor(
    string Name,
    string GeomertyResourceKey,
    Type ProviderType);
