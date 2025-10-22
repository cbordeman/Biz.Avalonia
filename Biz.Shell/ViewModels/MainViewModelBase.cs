using Biz.Modules.Dashboard.Core;
using Biz.Shell.Services.Authentication;
using ShadUI;

namespace Biz.Shell.ViewModels;

/// <summary>
/// This is not a page; it is the top level view which contains
/// the MainContentRegion, which hosts pages.  Desktop clients
/// do have a window above this view, though.
/// </summary>
public abstract class MainViewModelBase
    : NavigationAwareViewModelBase, IOnViewLoaded
{
    protected readonly IMainNavigationService MainContentNavigationService;
    protected readonly IAuthenticationService AuthService;
    
    #region IsDrawerOpen
    public bool IsDrawerOpen
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion IsDrawerOpen

    #region IsLoggedIn
    public bool IsLoggedIn
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion IsLoggedIn

    #region CurrentArea
    public string? CurrentArea
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion CurrentArea

    #region CurrentPage
    public PageViewModelBase? CurrentPage
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion CurrentPage

    #region CurrentMode
    public ThemeMode CurrentTheme
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion CurrentTheme

    public ToastManager ToastManager { get; }

    protected MainViewModelBase()
    {
        ToastManager = Locator.Current.Resolve<ToastManager>();

        AuthService = Locator.Current.Resolve<IAuthenticationService>();
        AuthService.AuthenticationStateChanged.Subscribe(() =>
        {
            IsLoggedIn = AuthService.IsAuthenticated;
            return Task.CompletedTask;
        });

        MainContentNavigationService =
            Locator.Current.Resolve<IMainNavigationService>();
        MainContentNavigationService.PageChanged.Subscribe( 
            MainContentRegionNavigationServiceOnPageChanged);
    }

    Task MainContentRegionNavigationServiceOnPageChanged(
        NotifyPageChangedArgs args)
    {
        CurrentArea = args.Area.Split('.').FirstOrDefault();
        CurrentPage = args.Page;
   
        return Task.CompletedTask;
    }

    #region SwitchThemeCommand
    [field: AllowNull, MaybeNull]
    public AsyncRelayCommand SwitchThemeCommand => field ??= new AsyncRelayCommand(ExecuteSwitchThemeCommand, CanSwitchThemeCommand);
    bool CanSwitchThemeCommand() => true;
    Task ExecuteSwitchThemeCommand()
    {
        ThemeVariant variant;
        switch (CurrentTheme)
        {
            case ThemeMode.System:
                variant = ThemeVariant.Light;
                CurrentTheme = ThemeMode.Light;
                break;
            case ThemeMode.Light:
                variant = ThemeVariant.Dark;
                CurrentTheme = ThemeMode.Dark;
                break;
            default:
                variant = ThemeVariant.Default;
                CurrentTheme = ThemeMode.System;
                break;
        }

        Dispatcher.UIThread.Invoke(() =>
            Application.Current!.RequestedThemeVariant = variant);
        //themeWatcher.SwitchTheme(CurrentTheme);

        return Task.CompletedTask;
    }
    #endregion SwitchThemeCommand

    #region ToggleIsDrawerOpenCommand
    [field: AllowNull, MaybeNull]
    public AsyncRelayCommand ToggleIsDrawerOpenCommand => field ??= new AsyncRelayCommand(ExecuteToggleIsDrawerOpenCommand, CanToggleIsDrawerOpenCommand);
    static bool CanToggleIsDrawerOpenCommand() => true;

    Task ExecuteToggleIsDrawerOpenCommand()
    {
        IsDrawerOpen = !IsDrawerOpen;
        return Task.CompletedTask;
    }
    #endregion ToggleIsDrawerOpenCommand

    #region NavigateSettingsCommand
    public AsyncRelayCommand? NavigateSettingsCommand => field ??= 
        new AsyncRelayCommand(ExecuteNavigateSettingsCommand, 
            CanNavigateSettingsCommand);
    static bool CanNavigateSettingsCommand() => true;
    async Task ExecuteNavigateSettingsCommand()
    {
        await MainContentNavigationService.NavigateWithModuleAsync(
            null, nameof(SettingsView));
    }
    #endregion NavigateSettingsCommand

    // #region GoToPageCommand
    // [field: AllowNull, MaybeNull]
    // public AsyncRelayCommandWithParam<string> 
    //     GoToPageCommand => field 
    //     ??= new AsyncRelayCommandWithParam<string>
    //     (ExecuteGoToPageCommand, CanGoToPageCommand);
    // static bool CanGoToPageCommand(string area) => true;
    // Task ExecuteGoToPageCommand(string area)
    // {
    //     if (CurrentArea != area)
    //         NavigationService.RequestNavigate(
    //             null, area);
    //     return Task.CompletedTask;
    // }
    // #endregion GoToPageCommand

    public override void Dispose()
    {
        base.Dispose();
        MainContentNavigationService.PageChanged.Subscribe(
            MainContentRegionNavigationServiceOnPageChanged);
    }

    public async void OnViewLoaded()
    {
        try
        {
            IsLoggedIn = AuthService.IsAuthenticated;

            // This executes after regions are loaded.
            MainContentNavigationService.Initialize();
        
            await MainContentNavigationService.NavigateWithModuleAsync(
                DashboardConstants.ModuleName,
                DashboardConstants.DashboardView);
        }
        catch (Exception e)
        {
            var logger = Locator.Current
                .Resolve<ILogger<MainViewModelBase>>();
            logger.LogError(e, 
                "In {ClassName}.{MethodName}: {EMessage}", 
                nameof(MainViewModelBase), nameof(OnViewLoaded), e.Message);
        }
    }

    #region LogoutCommand
    [field: AllowNull, MaybeNull]
    public AsyncRelayCommand LogoutCommand => field ??= new AsyncRelayCommand(ExecuteLogoutCommand, CanLogoutCommand);
    static bool CanLogoutCommand() => true;
    async Task ExecuteLogoutCommand()
    {
        // Opens browser to the sign out page to ensure cookies
        // are cleared and provider actions can execute.
        await AuthService.LogoutAsync(true, true);

        // Can't set to null because of a bug in the sidebar control.
        // Must set to non-null or the property change doesn't trigger
        // properly.
        CurrentArea = "STUPID BUG";
    }
    #endregion LogoutCommand
}