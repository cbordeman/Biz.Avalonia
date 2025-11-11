using Biz.Modules.AccountManagement.Core;
using Biz.Modules.AccountManagement.Core.Services.Authentication;
using Biz.Modules.Dashboard.Core;

namespace Biz.Shared.Services;

public class MainNavigationService :
    IMainNavigationService, IDisposable
{
    bool initialized;
    readonly ISectionNavigationService navigationService;
    IAuthenticationService AuthenticationService =>
        Locator.Current.Resolve<IAuthenticationService>();
    readonly IAuthenticationService authenticationService;
    //readonly IModuleManager moduleManager;

    // this is the full page url
    public string? CurrentPage { get; private set; }

    // this is just the first bit of the Area that
    // was passed to the IContextNavigationService.
    public string? CurrentArea { get; private set; }

    public AsyncEvent<NotifyPageChangedArgs> PageChanged { get; } = new();

    public MainNavigationService(
        IAuthenticationService authenticationService,
        //IModuleManager moduleManager,
        ISectionNavigationService navigationService)
    {
        this.authenticationService = authenticationService;
        //this.moduleManager = moduleManager;
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
            {
                // Clear history
                var nav = Locator.Current.Resolve<ISectionNavigationService>();
                nav.ClearHistory();
                
                // Redirect to login page
                await NavigateWithModuleAsync(
                    AccountManagementConstants.ModuleName,
                    AccountManagementConstants.LoginView);
            }
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
            Log.Logger.Error(exception, "In {ClassName}.{MethodName}()", nameof(AuthStateChanged), nameof(AuthStateChanged));
        }
    }

    async Task Navigated(NavigatedEventArgs args)
    {
        switch (args.Result)
        {
            case NavigationResult.Error:
                if (args.Result == NavigationResult.Error)
                    Log.Logger.Error(args.Exception, 
                        $"Navigation failed: " + 
                        $"{args.LocationName}");
                return;
        
            case NavigationResult.Cancelled:
                return;
            
            case NavigationResult.Success:
                try
                {
                    args.Context.ShouldNotBeNull();
                    args.Context.Location.ShouldNotBeNull();
            
                    CurrentPage = args.LocationName;
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
                    Log.Logger.Error(
                        e,
                        "In (1) {ClassName}.{MethodName}() url: {CtxUri}, Error: {EMessage}",
                        nameof(MainNavigationService),
                        nameof(Navigated),
                        args.LocationName,
                        e.Message);
                }

                try
                {
                    // We must re-check the page we're still authenticated on
                    // every navigation, even though we do check this on
                    // authentication events in AuthenticationService.
                    if (args.LocationName != 
                        AccountManagementConstants.LoginView &&
                        args.LocationName != AccountManagementConstants
                            .TenantSelectionView &&
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
                    Log.Logger.Error(
                        e,
                        "In (2) {ClassName}.{MethodName}() url: {CtxUri}, Error: {EMessage}",
                        nameof(MainNavigationService),
                        nameof(Navigated),
                        args.LocationName,
                        e.Message);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(args.Result.ToString());
        }
    }

    public async Task NavigateWithModuleAsync(string? module, string area,
        params NavParam[] parameters)
    {
        if (!initialized)
            throw new InvalidOperationException("Not initialized.");

        await navigationService.NavigateToAsync(module, area, parameters);
    }

    public void Dispose()
    {
        navigationService.Navigated.Unsubscribe(Navigated);
    }
}
