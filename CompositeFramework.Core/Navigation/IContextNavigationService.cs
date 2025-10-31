using System.ComponentModel;
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
    /// The context to which the service is bound, such as a
    /// section name.
    /// </summary>
    object? Context { get; set; }
    
    /// <summary>
    /// Invoked after any navigation event, even if cancelled
    /// or an exception is thrown.
    /// </summary>
    AsyncEvent<NavigatedEventArgs> Navigated { get; }
    
    /// <summary>
    /// Navigates to a new location.  Clears forward history.
    /// </summary>
    /// <param name="location"></param>
    /// <param name="parameters"></param>
    /// <returns>True if navigation was successful and not
    /// cancelled or disallowed.</returns>
    Task<NavigationResult> NavigateToAsync(string location, params NavParam[] parameters);
    
    /// <summary>
    /// Navigates backward.
    /// </summary>
    /// <param name="toLocation"></param>
    /// <returns></returns>
    Task<NavigationResult> GoBackAsync(ILocation? toLocation = null);
    Task<NavigationResult> GoForwardAsync(ILocation? toLocation = null);
    
    /// <summary>
    /// Clears all locations from history but the
    /// current location.
    /// </summary>
    void ClearHistory();

    /// <summary>
    /// Clears all locations from forward history.
    /// </summary>
    void ClearForwardHistory();
    
    /// <summary>
    /// All locations navigated to where AddToHistory is true.
    /// </summary>
    IReadOnlyCollection<ILocation> History { get; }
    
    /// <summary>
    /// Forward history where AddToHistory is true.
    /// Wiped on forward navigation to a new location.
    /// </summary>
    IReadOnlyCollection<ILocation> ForwardHistory { get; }
    
    ICommand NavigateForwardCommand { get; }
    ICommand NavigateBackCommand { get; }

    /// <summary>
    /// Associates a viewmodel with its view for navigation.  Does <i>not</i>
    /// register types with the container.
    /// </summary>
    /// <param name="locationName"></param>
    /// <typeparam name="TViewModel"></typeparam>
    /// <typeparam name="TView"></typeparam>
    void RegisterForNavigation<TViewModel, TView>
        (string? locationName = null)
        where TViewModel : INotifyPropertyChanged, ILocation;

    IReadOnlyDictionary<string, ViewModelLocationBinding> Registrations { get; }
}