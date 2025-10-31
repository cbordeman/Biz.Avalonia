// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Local

namespace CompositeFramework.Core.Navigation;

public interface ILocation
{
    Task OnNavigatedToAsync(NavigationContext context);

    /// <summary>
    /// Return false to cancel navigation.
    /// </summary>
    Task<bool> CanNavigateToAsync(NavigationContext navigationContext);
    
    /// <summary>
    /// Return false to disallow (NavigationResult.NotAllowed).
    /// </summary>
    /// <param name="newContext"></param>
    /// <returns></returns>
    Task<bool> OnNavigatingFromAsync(NavigationContext newContext); 
    Task OnNavigatedFromAsync(NavigationContext navigationContext);
    
    /// <summary>
    /// If true, a reference to the view is maintained in history
    /// until that history item is removed or history is cleared.
    /// The view reference exists even if AddToHistory is false.
    /// If false, view will be recreated when the page is navigated
    /// to if not a singleton in your container.
    /// <br/> 
    /// Defaults to false to reduce memory pressure.
    /// </summary>
    bool KeepViewAlive => false;
    
    /// <summary>
    /// Set to false to keep out of the normal flow of history,
    /// though the location will still be referenced internally.
    /// GoBack() and GoForward() will skip the location if called
    /// without a specific location.
    /// <br/>
    /// Used for login and other activities that are outside
    /// typical navigation flow.
    /// </summary>
    bool AddToHistory => true;
    
    /// <summary>
    /// Used to prevent forward navigation.  Referenced 
    /// by ICommand in IContextNavigationService.
    /// </summary>
    bool AllowForwardNavigation => false;
    
    /// <summary>
    /// Used to prevent back navigation.  Referenced 
    /// by ICommand in IContextNavigationService.
    /// </summary>
    bool AllowBackwardNavigation => true;

    /// <summary>
    /// Used in menus to highlight the current area. 
    /// </summary>
    public string Area { get; }

    /// <summary>
    /// Name the location was registered as.  Set by the navigation
    /// framework when the page is first navigated to.
    /// Do not set.
    /// </summary>
    public string LocationName { get; set; }

    /// <summary>
    /// Used to pass values during navigation.  Set by the navigation
    /// framework when the page is first navigated to.
    /// Do not set.
    /// </summary>
    public NavParam[] NavigationParameters { get; set; }
}