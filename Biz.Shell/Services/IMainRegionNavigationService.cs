namespace Biz.Shell.Services;

public delegate void NotifyPageChanged(string area, PageViewModelBase page);

public interface IMainRegionNavigationService
{
    string? CurrentPage { get; }
    event NotifyPageChanged? PageChanged;

    void Initialize();

    Task NavigateAsync(string module, string area, 
        IDictionary<string, object>? parameters = null);
   
}