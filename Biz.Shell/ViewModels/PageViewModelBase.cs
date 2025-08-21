using System.Collections.ObjectModel;
using Biz.Shell.Services;
using Biz.Shell.ViewModels.Toolbar;

namespace Biz.Shell.ViewModels;

public abstract class PageViewModelBase(IContainer container) 
    : NavigationAwareViewModelBase(container)
{
    protected IPlatformDialogService DialogService => Container.Resolve<IPlatformDialogService>();
    protected IAuthenticationService AuthenticationService => Container.Resolve<IAuthenticationService>();
    
    #region Title
    public string? Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }
    string? title;        
    #endregion Title

    #region TitleGeometryResourceName
    public string? TitleGeometryResourceName
    {
        get => titleGeometryResourceName;
        set => SetProperty(ref titleGeometryResourceName, value);
    }
    string? titleGeometryResourceName;        
    #endregion TitleGeometryResourceName
    
    #region IsMinimalUi
    public bool IsMinimalUi
    {
        get => isMinimalUi;
        set
        {
            if (SetProperty(ref isMinimalUi, value))
                RaisePropertyChanged(nameof(IsFullUi));
        }
    }
    bool isMinimalUi;
    public bool IsFullUi => !IsMinimalUi;
    #endregion MinimalUi

    public ObservableCollection<IToolbarEntry> ToolbarEntries { get; } = [];
}