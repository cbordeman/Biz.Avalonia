namespace Biz.Core.Services;

public delegate void NotifyMainAreaChanged(string area);

public interface IMainRegionNavigationService
{
    string? CurrentArea { get; }
    event NotifyMainAreaChanged? AreaChanged;

    void Initialize();
}