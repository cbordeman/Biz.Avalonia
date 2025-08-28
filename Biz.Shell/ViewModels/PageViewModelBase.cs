using Biz.Models;
using Biz.Shell.Services.Authentication;
using Biz.Shell.ViewModels.Toolbar;
using Microsoft.Extensions.Logging;

namespace Biz.Shell.ViewModels;

public abstract class PageViewModelBase : NavigationAwareViewModelBase
{
    protected IAuthenticationService AuthenticationService { get; }
    
    // This must be public so MainWindow can bind to its DialogHost property.
    public IPlatformDialogService DialogService { get; }
    
    #region Title
    public string? Title
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion Title

    #region TitleGeometryResourceName
    public string? TitleGeometryResourceName
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion TitleGeometryResourceName
    
    #region IsMinimalUi
    public bool IsMinimalUi
    {
        get => field;
        set
        {
            if (SetProperty(ref field, value))
                RaisePropertyChanged(nameof(IsFullUi));
        }
    }
    public bool IsFullUi => !IsMinimalUi;
    #endregion MinimalUi
    
    #region CurrentUser
    public User? CurrentUser
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion CurrentUser

    public ObservableCollection<IToolbarEntry> ToolbarEntries { get; } = [];

    protected PageViewModelBase(IContainer container) : base(container)
    {
        DialogService = Container.Resolve<IPlatformDialogService>();
        AuthenticationService = Container.Resolve<IAuthenticationService>();
        AuthenticationService.AuthenticationStateChanged += () =>
        {
            var logger = Container.Resolve<ILogger<PageViewModelBase>>();
            CurrentUser = AuthenticationService.GetCurrentUserAsync()
                .LogExceptionsBlockAndGetResult($"Getting user from " +
                              $"{nameof(PageViewModelBase)}", logger);
        };
    }
}