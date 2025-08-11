namespace Biz.Core.Services;

public delegate void NotifyPageChanged(string area, string route);

public interface IMainRegionNavigationService
{
    string? CurrentPageArea { get; }
    string? CurrentRoute { get; }
    event NotifyPageChanged? PageChanged;

    void Initialize();
}