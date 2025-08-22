using Biz.Shell.Services.Authentication;
using Biz.Shell.ViewModels.Toolbar;

namespace Biz.Shell.ViewModels;

public abstract class PageViewModelBase : NavigationAwareViewModelBase
{
    protected IAuthenticationService AuthenticationService { get; }
    public IPlatformDialogService DialogService { get; }
    
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

    protected PageViewModelBase(IContainer container) : base(container)
    {
        DialogService = Container.Resolve<IPlatformDialogService>();
        AuthenticationService = Container.Resolve<IAuthenticationService>();
    }
}