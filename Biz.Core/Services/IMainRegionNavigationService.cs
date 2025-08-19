using Biz.Core.ViewModels;

namespace Biz.Core.Services;

public delegate void NotifyPageChanged(string area, PageViewModelBase page);

public interface IMainRegionNavigationService
{
    string? CurrentPage { get; }
    event NotifyPageChanged? PageChanged;

    void Initialize();
}