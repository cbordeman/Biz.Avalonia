using System.Collections.Generic;
using System.ComponentModel;
using Biz.Modules.Dashboard;
using ShadUI;
using IContainer = DryIoc.IContainer;

namespace Biz.Shell.ViewModels;

public abstract class MainViewModelBase 
    : FormFactorAwareViewModel, IOnViewLoaded
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
        mainContentRegionNavigationService.AreaChanged += MainContentRegionNavigationServiceOnPageChanged;
    }
    
    void MainContentRegionNavigationServiceOnPageChanged(string area)
    {
        CurrentArea = area;
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
        mainContentRegionNavigationService.AreaChanged -=
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
}