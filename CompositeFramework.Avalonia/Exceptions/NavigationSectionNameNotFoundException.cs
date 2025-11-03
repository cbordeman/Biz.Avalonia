namespace CompositeFramework.Avalonia.Exceptions;

public class NavigationSectionNameNotFoundException : Exception
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public ISectionNavigationService? SectionNavigationServiceInstance { get; }
    
    public NavigationSectionNameNotFoundException(string sectionName,
        ISectionNavigationService? sectionNavigationServiceInstance = null)
        : base($"Navigation service instance's SectionName not " +
               $"found: {sectionName}.  Set in AXAML using " +
               $"SectionManager.SectionName = \"{sectionName}\". on " +
               $"a ContentControl or derived control.")
    {
        SectionNavigationServiceInstance = sectionNavigationServiceInstance;
    }
}
