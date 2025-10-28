using Biz.Shared.ViewModels;

namespace Biz.Shared.Services;

public record NotifyPageChangedArgs(string Area, PageViewModelBase Page);

public interface IMainNavigationService
{
    string? CurrentPage { get; }
    AsyncEvent<NotifyPageChangedArgs> PageChanged { get; }

    void Initialize();

    Task NavigateWithModuleAsync(string? module, string area, 
        params NavParam[] parameters);
}