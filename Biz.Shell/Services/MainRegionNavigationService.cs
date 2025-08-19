using Microsoft.Extensions.Logging;

namespace Biz.Shell.Services;

public class MainContentRegionNavigationService : IMainRegionNavigationService,
    IDisposable
{
    bool initialized;
    IRegionNavigationService? regionNavigationService;
    readonly IRegionManager regionManager;
    readonly ILogger<MainContentRegionNavigationService> logger;

    public string? CurrentPage { get; private set; }
    public string? CurrentArea { get; private set; }

    public event NotifyPageChanged? PageChanged;

    public MainContentRegionNavigationService(IRegionManager regionManager,
        ILogger<MainContentRegionNavigationService> logger)
    {
        this.regionManager = regionManager;
        this.logger = logger;
    }

    /// <summary>
    /// This must be called <i>after</i> the main region has been
    /// fully loaded.
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    public void Initialize()
    {
        if (initialized)
            return;

        if (!this.regionManager.Regions.ContainsRegionWithName(RegionNames.MainContentRegion))
            throw new InvalidOperationException(
                $"MainContentRegion not found.  Could not initialize {nameof(MainContentRegionNavigationService)}.");

        var mainRegion = this.regionManager.Regions[RegionNames.MainContentRegion];
        regionNavigationService = mainRegion.NavigationService;

        regionNavigationService.Navigated += Navigated;
        regionNavigationService.NavigationFailed += NavigationFailed;

        initialized = true;
        return;
    }

    void NavigationFailed(object? sender, RegionNavigationFailedEventArgs args)
    {
        logger.LogError(args.Error, $"Navigation failed: {args.Uri}");
    }

    void Navigated(object? sender, RegionNavigationEventArgs args)
    {
        try
        {
            CurrentPage = args.Uri.OriginalString;
            CurrentArea = args.Uri.OriginalString.Split('.').First();
            
            // There should be exactly one active view
            // in the main region.
            var pageView = regionNavigationService!.Region
                .ActiveViews.Single();
            if (pageView == null)
                throw new InvalidOperationException("Page contains no views.");
            if (pageView is not UserControl uc)
                throw new InvalidOperationException(
                    "Page view must be derived from UserControl");
            if (uc.DataContext is not PageViewModelBase vm)
                throw new InvalidOperationException(
                    "Page DataContext (ViewModel) must be derived from PageViewModelBase");
            else
                PageChanged?.Invoke(CurrentArea!, vm);
        }
        catch (Exception e)
        {
            logger.LogError(e.GetBaseException(),
                $"Failed in Navigation() method in {nameof(MainContentRegionNavigationService)}.");
            throw;
        }
    }

    public void Dispose()
    {
        if (regionNavigationService == null) return;

        regionNavigationService.Navigated -= Navigated;
        regionNavigationService.NavigationFailed -= NavigationFailed;
    }
}