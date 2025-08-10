namespace Biz.Core.Services;

public delegate void NotifyPageChanged(string area, string route);

public interface IMainContentRegionNavigationService
{
    string? CurrentPageArea { get; }
    string? CurrentRoute { get; }
    event NotifyPageChanged? PageChanged;

    void Initialize();
}