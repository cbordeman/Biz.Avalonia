using CompositeFramework.Avalonia.Navigation;
using Splat;

namespace CompositeFramework.Avalonia;

public static class CompositeAvaloniaInitializer
{
    /// <summary>
    /// Registers default service implementations for
    /// CompositeFramework.Avalonia.
    /// </summary>
    public static void RegisterDefaultServices()
    {
        SplatRegistrations.SetupIOC();
        
        SplatRegistrations.RegisterLazySingleton<IContextNavigationService, SectionNavigationService>();
    }
}
