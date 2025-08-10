namespace Biz.Shell.Infrastructure;

public class MainContentRegionNavigationService : IMainContentRegionNavigationService
{
    bool initialized;
    
    readonly IRegionManager regionManager;
    
    public string? CurrentPageArea { get; private set; }
    public string? CurrentRoute { get; private set; }
    
    public event NotifyPageChanged? PageChanged;

    public MainContentRegionNavigationService(IRegionManager regionManager)
    {
        this.regionManager = regionManager;
        
    }

    public void Initialize()
    {
        if (initialized) 
            return;

        if (this.regionManager.Regions.ContainsRegionWithName(RegionNames.MainContentRegion))
        {
            this.regionManager.Regions[RegionNames.MainContentRegion].NavigationService.Navigated +=
                OnNavigationServiceNavigated;
            initialized = true;
            return;
        }

        throw new InvalidOperationException(
            $"MainContentRegion not found.  Could not initialize {nameof(MainContentRegionNavigationService)}.");
    }
    
    void OnNavigationServiceNavigated(object? sender, 
        RegionNavigationEventArgs args)
    {
        var region = args.NavigationContext.NavigationService.Region;
        var view = region.ActiveViews.FirstOrDefault();
        if (view == null) 
            throw new InvalidOperationException("View must be active.");
        var viewType = view.GetType();
        var pageAttribute = viewType.GetCustomAttribute<PageAttribute>();
        if (pageAttribute == null) 
            throw new InvalidOperationException("View must have Page attribute with the area name.");

        CurrentPageArea = pageAttribute.AreaName; 
        CurrentRoute = args.Uri.AbsoluteUri;
        
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
}