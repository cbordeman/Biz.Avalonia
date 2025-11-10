using System.ComponentModel;
using System.Windows.Input;

namespace CompositeFramework.Core.Navigation;

/// <summary>
/// Instances of this service are created for different
/// sections, such as the main navigation area, a title, a tab,
/// or a dialog. 
/// </summary>
public interface ISectionNavigationService
{
    /// <summary>
    /// The section name to which this instance is bound.  Set
    /// via the Initialize() method.
    /// </summary>
    string? SectionName { get; }
    
    /// <summary>
    /// Invoked after any navigation event, even if cancelled
    /// or an exception is thrown.
    /// </summary>
    AsyncEvent<NavigatedEventArgs> Navigated { get; }

    /// <summary>
    /// Navigates to a new location.  Clears forward history.
    /// </summary>
    /// <param name="moduleName"></param>
    /// <param name="location"></param>
    /// <param name="parameters"></param>
    /// <returns>True if navigation was successful and not
    /// cancelled or disallowed.</returns>
    Task<NavigationResult> NavigateToAsync(
        string? moduleName, string location, params NavParam[] parameters);
    
    /// <summary>
    /// Navigates backward.
    /// </summary>
    /// <param name="toLocation"></param>
    /// <returns></returns>
    Task<NavigationResult> GoBackAsync();
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
    
    ILocation? CurrentLocation { get; }
    
    /// <summary>
    /// All locations navigated to where AddToHistory is true.
    /// Does not include CurrentLocation.
    /// </summary>
    ILocation[] History { get; }
    
    /// <summary>
    /// Forward history where AddToHistory is true.
    /// Wiped on forward navigation to a new location.
    /// </summary>
    ILocation[] ForwardHistory { get; }
    
    ICommand NavigateForwardCommand { get; }
    ICommand NavigateBackCommand { get; }

    /// <summary>
    /// Associates a viewmodel with its view for navigation.  Does <i>not</i>
    /// You must register types with the container.
    /// </summary>
    void RegisterForNavigation<TViewModel, TView>
        (string? locationName = null)
        where TViewModel : INotifyPropertyChanged, ILocation;

    IReadOnlyDictionary<string, ViewModelViewBinding> Registrations { get; }

    /// <summary>
    /// Call this before navigating.
    /// </summary>
    /// <param name="sectionName"></param>
    void Initialize(string sectionName);

    /// <summary>
    /// Refreshes the location.  If createNew is true, a new viewmodel
    /// and view are created.
    /// </summary>
    /// <param name="alternateLocationName">If the current page cannot
    /// be determined, this location will be navigated to.</param>
    /// <param name="alternativeModulename">The name of the modul to load
    /// for the alternative page.</param>
    /// <param name="createNew">If true, creates a new viewmodel
    /// and view.  If false, the view will be recreated if it is
    /// not not persisted by the navigation service,
    /// but the viewmodel will be reused.</param>
    /// <returns></returns>
    Task<NavigationResult> Refresh(
        string? alternativeModulename,
        string alternateLocationName,
        bool createNew = true);
}