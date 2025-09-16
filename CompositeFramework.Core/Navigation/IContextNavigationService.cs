using System.Windows.Input;

namespace CompositeFramework.Core.Navigation;

/// <summary>
/// Instances of This service can be created for different
/// contexts, such as the main navigation area, a title, a tab,
/// or a dialog.  
/// </summary>
public interface IContextNavigationService
{
    /// <summary>
    /// Invoked after any navigation event, even if cancelled
    /// or an exception is thrown.
    /// </summary>
    public event NavigationEventArgs Navigated;
    
    /// <summary>
    /// Navigates to a new location.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="parameters"></param>
    /// <returns>True if navigation was successful and not
    /// cancelled or disallowed.</returns>
    Task<bool> NavigateAsync(string location, 
        IDictionary<string, object>? parameters = null);
    
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
    
    /// <summary>
    /// Index of the current location.
    /// </summary>
    int? CurrentLocationIndex { get; }

    ICommand NavigateForwardCommand { get; }
    ICommand NavigateBackCommand { get; }
}
