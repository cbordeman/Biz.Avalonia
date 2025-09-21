namespace Biz.Shell.Services;

public record NotifyPageChangedArgs(string Area, PageViewModelBase Page);

public interface IMainRegionNavigationService
{
    string? CurrentPage { get; }
    AsyncEvent<NotifyPageChangedArgs> PageChanged { get; }

    void Initialize();

    Task NavigateWithModuleAsync(string? module, string area, 
        params NavParam[] parameters);
}