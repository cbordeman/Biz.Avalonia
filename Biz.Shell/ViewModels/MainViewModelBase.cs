using Biz.Modules.Dashboard;
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
    readonly IMainRegionNavigationService mainContentRegionNavigationService;

    #region IsDrawerOpen
    public bool IsDrawerOpen
    {
        get => isDrawerOpen;
        set => SetProperty(ref isDrawerOpen, value);
    }
    bool isDrawerOpen;        
    #endregion IsDrawerOpen
    
    #region CurrentArea
    public string? CurrentArea
    {
        get => currentArea;
        set => SetProperty(ref currentArea, value);
    }
    string? currentArea;        
    #endregion CurrentArea
    
    #region CurrentPage
    public PageViewModelBase? CurrentPage
    {
        get => currentPage;
        set => SetProperty(ref currentPage, value);
    }
    PageViewModelBase? currentPage;        
    #endregion CurrentPage
    
    #region CurrentMode
    public ThemeMode CurrentTheme
    {
        get => currentTheme;
        set => SetProperty(ref currentTheme, value);
    }
    ThemeMode currentTheme;        
    #endregion CurrentTheme
    
    public ToastManager ToastManager { get; }
    
    protected MainViewModelBase(IContainer container) : base(container)
    {
        ToastManager = container.Resolve<ToastManager>();

        mainContentRegionNavigationService = 
            container.Resolve<IMainRegionNavigationService>();
        mainContentRegionNavigationService.PageChanged += MainContentRegionNavigationServiceOnPageChanged;
    }
    
    void MainContentRegionNavigationServiceOnPageChanged(
        string area, PageViewModelBase pageVm)
    {
        CurrentArea = area;
        CurrentPage = pageVm;
    }

    #region SwitchThemeCommand
    AsyncDelegateCommand? switchThemeCommand;
    public AsyncDelegateCommand SwitchThemeCommand => switchThemeCommand ??= new AsyncDelegateCommand(ExecuteSwitchThemeCommand, CanSwitchThemeCommand);
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
    AsyncDelegateCommand? toggleIsDrawerOpenCommand;
    public AsyncDelegateCommand ToggleIsDrawerOpenCommand => toggleIsDrawerOpenCommand ??= new AsyncDelegateCommand(ExecuteToggleIsDrawerOpenCommand, CanToggleIsDrawerOpenCommand);
    static bool CanToggleIsDrawerOpenCommand() => true;

    Task ExecuteToggleIsDrawerOpenCommand()
    {
        IsDrawerOpen = !IsDrawerOpen;
        return Task.CompletedTask;
    }
    #endregion ToggleIsDrawerOpenCommand
    
    #region NavigateSettingsCommand
    AsyncDelegateCommand? navigateSettingsCommand;
    public AsyncDelegateCommand NavigateSettingsCommand => navigateSettingsCommand ??=
        new AsyncDelegateCommand(ExecuteNavigateSettingsCommand, CanNavigateSettingsCommand);
    static bool CanNavigateSettingsCommand() => true;
    Task ExecuteNavigateSettingsCommand()
    {
        this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(SettingsView));
        return Task.CompletedTask;
    }
    #endregion NavigateSettingsCommand
    
    #region GoToPageCommand
    AsyncDelegateCommandWithParam<string>? goToPageCommand;
    public AsyncDelegateCommandWithParam<string> GoToPageCommand => goToPageCommand 
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
        mainContentRegionNavigationService.PageChanged -=
            MainContentRegionNavigationServiceOnPageChanged;
    }
    
    public void OnViewLoaded()
    {
        // This executes after regions are loaded.
        mainContentRegionNavigationService.Initialize();
        
        // Have to load the module if it's not already loaded.
        var mm = Container.Resolve<ModuleManager>();
        mm.LoadModule(DashboardConstants.ModuleName);
        
        // Go to the dashboard initially.
        ExecuteGoToPageCommand(DashboardConstants.DashboardView);
    }
    
    #region LogoutCommand
    AsyncDelegateCommand? logoutCommand;
    public AsyncDelegateCommand LogoutCommand => logoutCommand ??= new AsyncDelegateCommand(ExecuteLogoutCommand, CanLogoutCommand);
    static bool CanLogoutCommand() => true;
    async Task ExecuteLogoutCommand()
    {
        var authService = Container.Resolve<IAuthenticationService>();
        authService.Logout(true);
        
        // Can't set to null because of a bug in the sidebar control.
        // Must set to non-null or the property change doesn't trigger
        // properly.
        CurrentArea = "STUPID BUG";
    }
    #endregion LogoutCommand
}