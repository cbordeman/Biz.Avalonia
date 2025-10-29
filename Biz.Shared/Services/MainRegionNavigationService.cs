using Biz.Modules.AccountManagement.Core;
using Biz.Modules.Dashboard.Core;
using Biz.Shared.Services.Authentication;

namespace Biz.Shared.Services;

public class MainNavigationService :
    IMainNavigationService, IDisposable
{
    bool initialized;
    readonly IContextNavigationService navigationService;
    IAuthenticationService AuthenticationService =>
        Locator.Current.Resolve<IAuthenticationService>();
    readonly ILogger<MainNavigationService> logger;
    readonly IAuthenticationService authenticationService;
    readonly IModuleManager moduleManager;

    // this is the full page url
    public string? CurrentPage { get; private set; }

    // this is just the first bit of the Area that
    // was passed to the IContextNavigationService.
    public string? CurrentArea { get; private set; }

    public AsyncEvent<NotifyPageChangedArgs> PageChanged { get; } = new();

    public MainNavigationService(
        ILogger<MainNavigationService> logger,
        IAuthenticationService authenticationService,
        IModuleManager moduleManager,
        IContextNavigationService navigationService)
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

        navigationService.Navigated.Subscribe(Navigated);

        authenticationService.AuthenticationStateChanged
            .Subscribe(AuthStateChanged);

        initialized = true;
    }

    async Task AuthStateChanged()
    {
        try
        {
            if (!await authenticationService.IsAuthenticated())
                // Redirect to login page
                await NavigateWithModuleAsync(
                    AccountManagementConstants.ModuleName,
                    AccountManagementConstants.LoginView);
            else
                // Go to dashboard.  Note that the region journal has been
                // emptied so there's no back history to go to.
                await NavigateWithModuleAsync(
                    DashboardConstants.ModuleName,
                    DashboardConstants.DashboardView);
        }
        catch (Exception exception)
        {
            // If we don't redirect to Login because of an exception, it
            // isn't catastrophic.  Server operations will simply fail
            // because of the lack of a token.
            logger.LogError(exception, "In {ClassName}.{MethodName}()", nameof(AuthStateChanged), nameof(AuthStateChanged));
        }
    }

    async Task Navigated(NavigatedEventArgs args)
    {
        if (args.Result != NavigationResult.Success)
        {
            if (args.Result == NavigationResult.Error)
                logger.LogError(args.Error, $"Navigation failed: " +
                                            $"{args.Context.ViewName}");
            return;
        }

        try
        {
            args.Context.Location.ShouldNotBeNull();
            
            CurrentPage = args.Context.ViewName;
            CurrentArea = args.Context.Location.Area;

            if (args.Context.Location is not PageViewModelBase vm)
                throw new InvalidOperationException(
                    "Page DataContext must be derived from PageViewModelBase");
            else
                await PageChanged.PublishSequentiallyAsync(
                    new NotifyPageChangedArgs(CurrentArea!, vm));
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "In (1) {ClassName}.{MethodName}() url: {CtxUri}, Error: {EMessage}",
                nameof(MainNavigationService),
                nameof(Navigated),
                args.Context.ViewName,
                e.Message);
        }

        try
        {
            // We must re-check the page we're still authenticated on
            // every navigation, even though we do check this on
            // authentication events in AuthenticationService.
            if (args.Context.ViewName != 
                    AccountManagementConstants.LoginView &&
                args.Context.ViewName != 
                    AccountManagementConstants.TenantSelectionView &&
                !await AuthenticationService.IsAuthenticated())
            {
                // Redirect to login page
                await NavigateWithModuleAsync(
                    AccountManagementConstants.ModuleName,
                    AccountManagementConstants.LoginView);
            }
        }
        catch (Exception e)
        {
            logger.LogError(
                e,
                "In (2) {ClassName}.{MethodName}() url: {CtxUri}, Error: {EMessage}",
                nameof(MainNavigationService),
                nameof(Navigated),
                args.Context.ViewName,
                e.Message);
        }
    }

    public async Task NavigateWithModuleAsync(string? module, string area,
        params NavParam[] parameters)
    {
        if (!initialized)
            throw new InvalidOperationException("Not initialized.");

        if (module != null)
            await moduleManager.LoadModuleAsync(module);
        
        await navigationService.NavigateToAsync(area, parameters);
    }

    public void Dispose()
    {
        navigationService.Navigated.Unsubscribe(Navigated);
    }
}
