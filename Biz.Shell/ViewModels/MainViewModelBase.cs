using ShadUI;

namespace Biz.Shell.ViewModels;

public abstract class MainViewModelBase : FormFactorAwareViewModel,
    IOnViewLoaded
{
    readonly ThemeWatcher themeWatcher;
    readonly IMainNavigationService mainContentRegionNavigationService;

    #region CurrentPageArea
    public string? CurrentPageArea
    {
        get => currentPageArea;
        set
        {
            if (SetProperty(ref currentPageArea, value))
            {
                GoToPageCommand.RaiseCanExecuteChanged();
            }
        }
    }

    string? currentPageArea;        
    #endregion CurrentPageArea

    #region CurrentRoute
    public string? CurrentRoute
    {
        get => currentRoute;
        set => SetProperty(ref currentRoute, value);
    }
    string? currentRoute;        
    #endregion CurrentRoute
    
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
        themeWatcher = container.Resolve<ThemeWatcher>();
        ToastManager = container.Resolve<ToastManager>();

        mainContentRegionNavigationService = 
            container.Resolve<IMainNavigationService>();
        mainContentRegionNavigationService.PageChanged += MainContentRegionNavigationServiceOnPageChanged;
    }

    void MainContentRegionNavigationServiceOnPageChanged(string area, string route)
    {
        CurrentPageArea = area;
        CurrentRoute = route;
        GoToPageCommand.RaiseCanExecuteChanged();
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
    bool CanGoToPageCommand(string area)
    {
        // This command can only be used with no route.  Use a custom
        // command if you need a route.
        return CurrentPageArea != area || CurrentRoute != null;
    }
    Task ExecuteGoToPageCommand(string area)
    {
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
        // After regions are loaded.
        mainContentRegionNavigationService.Initialize();
    }
}