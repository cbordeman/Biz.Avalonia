using System.Windows.Input;

namespace CompositeFramework.Core.Location;

public interface IKeepAlive
{
    bool KeepAlive { get; }
}

public interface IParticipateInHistory
{
    bool AddToHistory { get; }
}

public interface ILocationDescription
{
    string LocationDescription => string.Empty;
}

/// <summary>
/// Apply to the view or page to prevent navigation.
/// </summary>
public interface IAllowNavigation
{
    /// <summary>
    /// Return false to disallow navigation.
    /// </summary>
    /// <param name="newContext"></param>
    /// <returns></returns>
    Task<bool> CanNavigateToAsync(INagivationContext newContext) 
        => Task.FromResult(true);
    
    bool AllowForwardNavigation => true;
    bool AllowBackwardNavigation => true;
}

public interface INavigationAware
{
    /// <summary>
    /// Called when navigated to.  Use CanNavigateToAsync() to prevent
    /// navigation. 
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task OnNavigatedToAsync(INagivationContext context);
    
    Task<bool> OnNavigatingFromAsync(INagivationContext newContext) 
        => Task.FromResult(true);
    Task OnNavigatedAwayAsync(INagivationContext newContext);
    string LocationName => GetType().Name;
}

public interface INagivationContext
{
    bool IsBackNavigation { get; }
    INavigationAware Location { get; }
    IDictionary<string, object> Parameters { get; }
}

public interface IContextNavigationService
{
    Task<bool> NavigateToAsync(string location, 
        params IDictionary<string, object> parameters);
    Task<bool> GoBackAsync(INavigationAware? toLocation = null);
    void ClearHistory();
    IReadOnlyCollection<INavigationAware> History { get; }
    INavigationAware? CurrentLocation { get; }
    
    ICommand NavigateForwardCommand { get; }
    ICommand NavigateBackCommand { get; }
}