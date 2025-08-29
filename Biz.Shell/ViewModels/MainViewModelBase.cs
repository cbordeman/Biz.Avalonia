using System.Diagnostics.CodeAnalysis;
using Biz.Modules.Dashboard;
using Biz.Modules.Dashboard.Core;
using Biz.Shell.Services.Authentication;
using ShadUI;
using IContainer = DryIoc.IContainer;

namespace Biz.Shell.ViewModels;

/// <summary>
/// This is not a page; it is the top level view which contains
/// the MainContentRegion, which hosts pages.  Desktop clients
/// do have a window above this view, though.
/// </summary>
public abstract class MainViewModelBase 
    : NavigationAwareViewModelBase, IOnViewLoaded
{
    protected readonly IMainRegionNavigationService MainContentRegionNavigationService;
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
    
    protected MainViewModelBase(IContainer container) : base(container)
    {
        ToastManager = container.Resolve<ToastManager>();

        AuthService = container.Resolve<IAuthenticationService>();
        AuthService.AuthenticationStateChanged += () =>
        {
            IsLoggedIn = AuthService.IsAuthenticated;
        };
        IsLoggedIn = AuthService.IsAuthenticated;
        
        MainContentRegionNavigationService = 
            container.Resolve<IMainRegionNavigationService>();
        MainContentRegionNavigationService.PageChanged += MainContentRegionNavigationServiceOnPageChanged;
    }
    
    void MainContentRegionNavigationServiceOnPageChanged(
        string area, PageViewModelBase pageVm)
    {
        CurrentArea = area;
        CurrentPage = pageVm;
    }

    #region SwitchThemeCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand SwitchThemeCommand => field ??= new AsyncDelegateCommand(ExecuteSwitchThemeCommand, CanSwitchThemeCommand);
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
    public AsyncDelegateCommand ToggleIsDrawerOpenCommand => field ??= new AsyncDelegateCommand(ExecuteToggleIsDrawerOpenCommand, CanToggleIsDrawerOpenCommand);
    static bool CanToggleIsDrawerOpenCommand() => true;

    Task ExecuteToggleIsDrawerOpenCommand()
    {
        IsDrawerOpen = !IsDrawerOpen;
        return Task.CompletedTask;
    }
    #endregion ToggleIsDrawerOpenCommand
    
    #region NavigateSettingsCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand NavigateSettingsCommand => field ??=
        new AsyncDelegateCommand(ExecuteNavigateSettingsCommand, CanNavigateSettingsCommand);
    static bool CanNavigateSettingsCommand() => true;
    Task ExecuteNavigateSettingsCommand()
    {
        this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(SettingsView));
        return Task.CompletedTask;
    }
    #endregion NavigateSettingsCommand
    
    #region GoToPageCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommandWithParam<string> GoToPageCommand => field 
        ??= new AsyncDelegateCommandWithParam<string>
        (ExecuteGoToPageCommand, CanGoToPageCommand);
    static bool CanGoToPageCommand(string area) => true;
    Task ExecuteGoToPageCommand(string area)
    {
        if (CurrentArea != area)
            RegionManager.RequestNavigate(RegionNames.MainContentRegion, area);
        return Task.CompletedTask;
    }
    #endregion GoToPageCommand

    public override void Dispose()
    {
        base.Dispose();
        MainContentRegionNavigationService.PageChanged -=
            MainContentRegionNavigationServiceOnPageChanged;
    }
    
    public void OnViewLoaded()
    {
        // This executes after regions are loaded.
        MainContentRegionNavigationService.Initialize();
        
        // Have to load the module if it's not already loaded.
        var mm = Container.Resolve<ModuleManager>();
        mm.LoadModule(DashboardConstants.ModuleName);
        
        // Go to the dashboard initially.
        ExecuteGoToPageCommand(DashboardConstants.DashboardView);
    }
    
    #region LogoutCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand LogoutCommand => field ??= new AsyncDelegateCommand(ExecuteLogoutCommand, CanLogoutCommand);
    static bool CanLogoutCommand() => true;
    Task ExecuteLogoutCommand()
    {
        AuthService.Logout(true);
        
        // Can't set to null because of a bug in the sidebar control.
        // Must set to non-null or the property change doesn't trigger
        // properly.
        CurrentArea = "STUPID BUG";
        return Task.CompletedTask;
    }
    #endregion LogoutCommand
}