namespace Biz.Core;

public static class PlatformHelper
{
    public static IPlatformRegistrationService? RegistrationService { get; set; }
}

public interface IPlatformRegistrationService
{
    void RegisterPlatformTypes(IContainerRegistry containerRegistry);
}