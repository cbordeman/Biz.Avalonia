using Biz.Authentication;
using Biz.Shared.ViewModels.Toolbar;

// ReSharper disable UnusedMember.Global

namespace Biz.Shared.ViewModels;

public abstract class PageViewModelBase : NavigationAwareViewModelBase
{
    public IAuthenticationService AuthService { get; }

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

    public ObservableCollection<IToolbarEntry> ToolbarEntries { get; } = [];

    protected PageViewModelBase()
    {
        AuthService = Locator.Current
            .Resolve<IAuthenticationService>();
    }
}