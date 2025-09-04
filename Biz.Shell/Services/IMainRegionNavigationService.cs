namespace Biz.Shell.Services;

public delegate void NotifyPageChanged(string area, PageViewModelBase page);

public interface IMainRegionNavigationService
{
    string? CurrentPage { get; }
    event NotifyPageChanged? PageChanged;

    void Initialize();

    void ClearHistory();

    Task NavigateAsync(string module, string area, 
        INavigationParameters? navigationParameters = null);
    
    void RequestNavigate(string? module, string area,
        INavigationParameters? navigationParameters = null);
}