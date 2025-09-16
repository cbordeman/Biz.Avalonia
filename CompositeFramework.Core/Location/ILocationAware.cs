using System.Windows.Input;

namespace CompositeFramework.Core.Location;

public interface ILocationDescription
{
    string LocationDescription => string.Empty;
}

/// <summary>
/// Apply to the viewmodel prevent navigation.
/// </summary>
public interface IAllowNavigation
{
    /// <summary>
    /// Return false to disallow navigation.
    /// </summary>
    /// <param name="fromContext"></param>
    /// <returns></returns>
    Task<bool> CanNavigateToAsync(INavigationContext fromContext) 
        => Task.FromResult(true);
    
    /// <summary>
    /// Used to prevent forward navigation.  Referenced 
    /// by ICommand in IContextNavigationService.
    /// </summary>
    bool AllowForwardNavigation => true;
    
    /// <summary>
    /// Used to prevent back navigation.  Referenced 
    /// by ICommand in IContextNavigationService.
    /// </summary>
    bool AllowBackwardNavigation => true;
}

public interface INavigationAware
{
    /// <summary>
    /// Called when navigated to, forward or backward.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task OnNavigatedToAsync(INavigationContext context);

    /// <summary>
    /// Called just before navigating away from location.  Use
    /// for cleanup or saving state.
    /// </summary>
    /// <param name="newContext"></param>
    /// <returns></returns>
    Task OnNavigatingFromAsync(INavigationContext newContext) 
        => Task.FromResult(true);
    
    /// <summary>
    /// Defaults to GetType().Name.  Can be Used in breadcrumbs,
    /// and when navigating back to target a certain location. 
    /// </summary>
    string LocationName => GetType().Name;
    
    /// <summary>
    /// INavigationAware ViewModels are always kept alive by
    /// IContextNavigationService.History.  If true, references to
    /// views are also.  If false, views are no longer
    /// referenced after navigating away.  Set to true to
    /// retain view state.
    /// <br/>
    /// Defaults to false to reduce memory pressure.
    /// </summary>
    bool KeepViewAlive => false;
    
    /// <summary>
    /// Set to false to not add location to
    /// IContextNavigationService.History.  GoBack() will not
    /// see the location.  Used for login and other activities
    /// that are outside the normal navigation flow.
    /// </summary>
    bool AddToHistory => true;
}

public interface INavigationContext
{
    /// <summary>
    /// True when navigation is backward.
    /// </summary>
    bool IsBack { get; }
    
    /// <summary>
    /// The location being navigated to or from.
    /// </summary>
    Type? Location { get; }
    
    /// <summary>
    /// Used to pass values during navigation.
    /// </summary>
    IDictionary<string, object> Parameters { get; }
}

public delegate Task NavigationEvent(INavigationContext context);

/// <summary>
/// Instances of This service can be created for different
/// contexts, such as the main navigation area, a title, a tab,
/// or a dialog.  
/// </summary>
public interface IContextNavigationService
{
    public event NavigationEvent Navigated;
    
    /// <summary>
    /// Navigates to a new location.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="parameters"></param>
    /// <returns>True if navigation was successful and not
    /// cancelled or disallowed.</returns>
    Task<bool> NavigateToAsync(string location, 
        params IDictionary<string, object> parameters);
    
    /// <summary>
    /// Navigates backward.
    /// </summary>
    /// <param name="toLocation"></param>
    /// <returns></returns>
    Task<bool> GoBackAsync(INavigationAware? toLocation = null);
    
    /// <summary>
    /// Clears all locations from history.
    /// </summary>
    void ClearHistory();

    /// <summary>
    /// All locations navigated to where
    /// INavigationAware.AddToHistory is true.
    /// </summary>
    INavigationAware[] History { get; }
    
    int CurrentLocationIndex { get; }

    ICommand NavigateForwardCommand { get; }
    ICommand NavigateBackCommand { get; }
}

public record NavigationResult(bool Success, string? ErrorMessage = null);