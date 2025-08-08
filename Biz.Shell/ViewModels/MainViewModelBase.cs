using Avalonia.Styling;
using Avalonia.Threading;
using ShadUI;

namespace Biz.Shell.ViewModels;

public abstract class MainViewModelBase : NavigationAwareViewModelBase
{
    readonly ThemeWatcher themeWatcher;
    
    #region CurrentMode
    public ThemeMode CurrentTheme
    {
        get => currentTheme;
        set => SetProperty(ref currentTheme, value);
    }
    ThemeMode currentTheme;        
    #endregion CurrentTheme
    
    protected MainViewModelBase(IContainer container) : base(container)
    {
        themeWatcher = container.Resolve<ThemeWatcher>();
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

    bool CanNavigateSettingsCommand() => true;

    Task ExecuteNavigateSettingsCommand()
    {
        this.RegionManager.RequestNavigate(RegionNames.MainContentRegion, nameof(SettingsView));
        return Task.CompletedTask;
    }

    #endregion NavigateSettingsCommand
}