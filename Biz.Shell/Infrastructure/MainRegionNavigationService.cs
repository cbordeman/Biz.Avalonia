using Microsoft.Extensions.Logging;

namespace Biz.Shell.Infrastructure;

public class MainContentRegionNavigationService : IMainRegionNavigationService,
    IDisposable
{
    bool initialized;
    IRegionNavigationService? regionNavigationService;
    readonly IRegionManager regionManager;
    readonly ILogger<MainContentRegionNavigationService> logger;

    public string? CurrentPageArea { get; private set; }
    public string? CurrentRoute { get; private set; }
    
    public event NotifyPageChanged? PageChanged;

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
        // Use OriginalString so it also works with relative URIs
        var raw = args.Uri.OriginalString;

        string path;
        string query;

        if (args.Uri.IsAbsoluteUri)
        {
            path = args.Uri.AbsolutePath; // e.g. /Area/Route/More
            query = args.Uri.Query;       // e.g. ?id=1
        }
        else
        {
            int qIdx = raw.IndexOf('?');
            path = qIdx >= 0 ? raw[..qIdx] : raw;
            query = qIdx >= 0 ? raw[qIdx..] : string.Empty;
        }

        path = path.TrimStart('/');

        string pageArea;
        string route;

        int slashIdx = path.IndexOf('/');
        if (slashIdx >= 0)
        {
            pageArea = path[..slashIdx];
            var routePath = path[(slashIdx + 1)..];
            route = string.IsNullOrEmpty(query) ? routePath : routePath + query;
        }
        else
        {
            pageArea = path;
            // No additional path segments; keep route empty (or include query if you prefer)
            route = string.IsNullOrEmpty(query) ? string.Empty : query;
        }

        CurrentPageArea = pageArea;
        CurrentRoute = route;
        
        PageChanged?.Invoke(CurrentPageArea!, CurrentRoute!);
    }

    public void NavigateTo(string pageArea, string route,
        INavigationParameters? parameters = null)
    {
        CurrentPageArea = pageArea;
        CurrentRoute = route;
        this.regionManager.RequestNavigate(RegionNames.MainContentRegion, 
            route, parameters);
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