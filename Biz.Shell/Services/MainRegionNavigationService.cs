using Biz.Modules.Dashboard;
using Biz.Shell.Services.Authentication;
using Microsoft.Extensions.Logging;

namespace Biz.Shell.Services;

public class MainContentRegionNavigationService : 
    IMainRegionNavigationService, IDisposable
{
    bool initialized;
    IRegionNavigationService? regionNavigationService;
    IAuthenticationService AuthenticationService => ContainerLocator.Current.Resolve<IAuthenticationService>();
    readonly IRegionManager regionManager;
    readonly ILogger<MainContentRegionNavigationService> logger;
    readonly IAuthenticationService authenticationService;

    // this is the full page url
    public string? CurrentPage { get; private set; }
    
    // this is just the first bit
    public string? CurrentArea { get; private set; }

    public event NotifyPageChanged? PageChanged;

    public MainContentRegionNavigationService(IRegionManager regionManager,
        ILogger<MainContentRegionNavigationService> logger,
        IAuthenticationService authenticationService)
    {
        this.regionManager = regionManager;
        this.logger = logger;
        this.authenticationService = authenticationService;
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

        if (!regionManager.Regions.ContainsRegionWithName(RegionNames.MainContentRegion))
            throw new InvalidOperationException(
                $"MainContentRegion not found.  Could not initialize {nameof(MainContentRegionNavigationService)}.");

        var mainRegion = regionManager.Regions[RegionNames.MainContentRegion];
        regionNavigationService = mainRegion.NavigationService;
        
        regionNavigationService.Navigated += Navigated;
        regionNavigationService.NavigationFailed += NavigationFailed;

        authenticationService.AuthenticationStateChanged += AuthStateChanged;
        
        initialized = true;
    }
    async void AuthStateChanged(object? sender, bool e)
    {
        try
        {
            if (!await authenticationService.IsAuthenticatedAsync())
                RequestNavigate(nameof(LoginView), null);
            else
            {
                // TODO: Maybe this should ga back in the navigation stack
                // or do something smarter.
                RequestNavigate(nameof(DashboardConstants.DashboardView), null);
            }
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "In {ClassName}.{MethodName}()", nameof(AuthStateChanged), nameof(AuthStateChanged));       
        }
    }

    void NavigationFailed(object? sender, RegionNavigationFailedEventArgs args) =>
        logger.LogError(args.Error, $"Navigation failed: {args.Uri}");

    // ReSharper disable once AsyncVoidMethod
    async void Navigated(object? sender, RegionNavigationEventArgs args)
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
            logger.LogError(
                e, 
                "In (1) {ClassName}.{MethodName}() url: {CtxUri}, Error: {EMessage}", 
                nameof(MainContentRegionNavigationService), nameof(Navigated), 
                args.Uri, e.Message);
        }
        
        try
        {
            var name = args.NavigationContext.Uri.OriginalString;
            if (name != nameof(LoginView) &&
                name != nameof(TenantSelectionView) &&
                !await AuthenticationService.IsAuthenticatedAsync())
            {
                // Redirect to login page
                regionNavigationService
                    .RequestNavigate(nameof(LoginView));
            }
        }
        catch (Exception e)
        {
            logger.LogError(
                e, 
                "In (2) {ClassName}.{MethodName}() url: {CtxUri}, Error: {EMessage}", 
                nameof(MainContentRegionNavigationService), nameof(Navigated), 
                args.Uri, e.Message);
        }
    }

    public void ClearHistory()
    {
        if (!initialized)
            throw new InvalidOperationException("Not initialized.");
        regionNavigationService!.Journal.Clear();        
    }
    
    public void RequestNavigate(string area, INavigationParameters? navigationParameters)
    {
        if (!initialized)
            throw new InvalidOperationException("Not initialized.");
        if (navigationParameters == null)
            regionNavigationService!.RequestNavigate(area);
        else
            regionNavigationService!.RequestNavigate(area, navigationParameters);       
    }

    public void Dispose()
    {
        if (regionNavigationService == null) return;

        regionNavigationService.Navigated -= Navigated;
        regionNavigationService.NavigationFailed -= NavigationFailed;
    }
}