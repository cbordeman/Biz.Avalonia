using Microsoft.Extensions.Logging;

namespace Biz.Shell.Infrastructure;

public class MainContentRegionNavigationService : IMainRegionNavigationService,
    IDisposable
{
    bool initialized;
    IRegionNavigationService? regionNavigationService;
    readonly IRegionManager regionManager;
    readonly ILogger<MainContentRegionNavigationService> logger;

    public string? CurrentArea { get; private set; }
    
    public event NotifyMainAreaChanged? AreaChanged;

    public MainContentRegionNavigationService(IRegionManager regionManager,
        ILogger<MainContentRegionNavigationService> logger)
    {
        this.regionManager = regionManager;
        this.logger = logger;
    }

    /// <summary>
    /// This must be callled <i>after</i> the main region has been
    /// fully loaded.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Initialize()
    {
        if (initialized) 
            return;

        if (this.regionManager.Regions.ContainsRegionWithName(RegionNames.MainContentRegion))
        {
            var mainRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
            regionNavigationService = mainRegion.NavigationService;
            regionNavigationService.Navigated += Navigated;
            regionNavigationService.NavigationFailed += NavigationFailed;
            
            initialized = true;
            return;
        }
        throw new InvalidOperationException(
            $"MainContentRegion not found.  Could not initialize {nameof(MainContentRegionNavigationService)}.");
    }

    void NavigationFailed(object? sender, RegionNavigationFailedEventArgs args)
    {
        logger.LogError(args.Error, $"Navigation failed: {args.Uri}");
    }

    void Navigated(object? sender, RegionNavigationEventArgs args)
    {
        CurrentArea = args.Uri.OriginalString;
        AreaChanged?.Invoke(CurrentArea!);
    }
    
    public void Dispose()
    {
        if (regionNavigationService != null)
        {
            regionNavigationService.Navigated -= Navigated;
            regionNavigationService.NavigationFailed -= NavigationFailed;
        }
    }
}