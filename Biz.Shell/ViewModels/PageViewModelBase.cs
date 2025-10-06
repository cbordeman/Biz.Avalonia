using Biz.Shell.Services.Authentication;
using Biz.Shell.ViewModels.Toolbar;

// ReSharper disable UnusedMember.Global

namespace Biz.Shell.ViewModels;

public abstract class PageViewModelBase : NavigationAwareViewModelBase
{
    protected IAuthenticationService AuthenticationService { get; }

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

    protected PageViewModelBase()
    {
        AuthenticationService = Locator.Current
            .Resolve<IAuthenticationService>();
        AuthenticationService.AuthenticationStateChanged
            .Subscribe(OnAuthStateChanged); 
    }

    private async Task OnAuthStateChanged()
    {
        CurrentUser = await AuthenticationService
            .GetCurrentUserAsync();
    }
}