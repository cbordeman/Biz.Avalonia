using Biz.Modules.AccountManagement.Core;
using Biz.Modules.Dashboard.Core;
using Biz.Shell.Services.Authentication;
using Microsoft.Extensions.Logging;

namespace Biz.Shell.Services;

public class MainContentRegionNavigationService : 
    IMainRegionNavigationService, IDisposable
{
    bool initialized;
    readonly IContextNavigationService? navigationService;
    readonly IAuthenticationService AuthenticationService => 
        Locator.Current.Resolve<IAuthenticationService>();
    readonly ILogger<MainContentRegionNavigationService> logger;
    readonly IAuthenticationService authenticationService;
    readonly IModuleManager moduleManager;

    // this is the full page url
    public string? CurrentPage { get; private set; }
    
    // this is just the first bit
    public string? CurrentArea { get; private set; }

    public event NotifyPageChanged? PageChanged;

    public MainContentRegionNavigationService(
        ILogger<MainContentRegionNavigationService> logger,
        IAuthenticationService authenticationService,
        IModuleManager moduleManager, IContextNavigationService? navigationService)
    {
        this.logger = logger;
        this.authenticationService = authenticationService;
        this.moduleManager = moduleManager;
        this.navigationService = navigationService;
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

        navigationService.Navigated += NavigationServiceOnNavigated;
        
        authenticationService.AuthenticationStateChanged += AuthStateChanged;
        
        initialized = true;
    }
    
    void AuthStateChanged()
    {
        try
        {
            if (!authenticationService.IsAuthenticated)
                // Redirect to login page
                RequestNavigate(
                    AccountManagementConstants.ModuleName,
                    AccountManagementConstants.LoginView);
            else
                // Go to dashboard.  Note that the region journal has been
                // emptied so there's no back history to go to.
                RequestNavigate(
                    DashboardConstants.ModuleName,
                    nameof(DashboardConstants.DashboardView));
        }
        catch (Exception exception)
        {
            // If we don't redirect to Login because of an exception, it
            // isn't catastrophic.  Server operations will simply fail
            // because of the lack of a token.
            logger.LogError(exception, "In {ClassName}.{MethodName}()", nameof(AuthStateChanged), nameof(AuthStateChanged));       
        }
    }

    // ReSharper disable once AsyncVoidMethod
    async Task NavigationServiceOnNavigated(
        NavigationResult result,
        string error,
        NavigationContext context)
    {
        if (result != NavigationResult.Success)
            logger.LogError(error, $"Navigation failed: {context.Location?.Name ?? "null"}");
        try
        {
            CurrentPage = context.Location?.Name;
            CurrentArea = context.Location?.Area.Split('.').FirstOrDefault();
            
            // There should be exactly one active view
            // in the main region.
            var pageView = navigationService!.Region
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
            if (name != AccountManagementConstants.LoginView &&
                name != AccountManagementConstants.TenantSelectionView &&
                !AuthenticationService.IsAuthenticated)
            {
                // Redirect to login page
                RequestNavigate(
                    AccountManagementConstants.ModuleName,
                    AccountManagementConstants.LoginView);
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

    public async Task NavigateAsync(string? module, string area,
        params IDictionary<string, object> navigationParameters)
    {
        if (!initialized)
            throw new InvalidOperationException("Not initialized.");
        
        TaskCompletionSource<bool> tcs = new();
        
        if (module != null)
            await moduleManager.LoadModuleAsync(module);

        if (navigationParameters.Count > 0)
            navigationService!.RequestNavigate(area,
                NavigationCallback);
        else
            navigationService!.RequestNavigate(area, 
                NavigationCallback,
                navigationParameters);

        return tcs.Task;
        
        void NavigationCallback(NavigationResult nr)
        {
            if (nr.Success)
                tcs.SetResult(true);
            else if (nr.Exception != null)
                tcs.SetException(nr.Exception);
            else
                tcs.SetCanceled();
        }
    }

    public void RequestNavigate(string? module, string area,
        INavigationParameters? navigationParameters = null)
    {
        if (!initialized)
            throw new InvalidOperationException("Not initialized.");
        
        if (module != null)
            moduleManager.LoadModule(module);
#if DEBUG
        else
            logger.LogWarning("{MethodName}(null, {Area}) called with null module.", nameof(RequestNavigate), area);
#endif
        
        if (navigationParameters == null)
            navigationService!.RequestNavigate(area);
        else
            navigationService!.RequestNavigate(area, 
                navigationParameters);
    }

    public void Dispose()
    {
        if (navigationService == null) return;

        navigationService.Navigated -= Navigated;
        navigationService.NavigationFailed -= NavigationFailed;
    }
}