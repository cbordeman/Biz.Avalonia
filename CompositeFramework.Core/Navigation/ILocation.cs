// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Local

namespace CompositeFramework.Core.Navigation;

public interface ILocation
{
    /// <summary>
    /// Called when navigated to, forward or backward.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    Task OnNavigatedToAsync(NavigationContext context);

    /// <summary>
    /// Called just before navigating away from location.  Use
    /// for cleanup or saving state.
    /// </summary>
    /// <param name="newContext"></param>
    /// <returns></returns>
    Task OnNavigatingFromAsync(NavigationContext newContext) 
        => Task.CompletedTask;
    
    /// <summary>
    /// If true, the View is referenced, keeping views
    /// alive after navigating away.  If false, view is
    /// not referenced by the navigation manager and will
    /// be recreated when the page is navigated to.
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
    
    /// <summary>
    /// Return false to disallow navigation.  Surfaced as
    /// NavigationResult.NotAllowed.
    /// </summary>
    /// <param name="fromContext"></param>
    /// <returns></returns>
    Task<bool> CanNavigateToAsync(NavigationContext fromContext) 
        => Task.FromResult(true);
    
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
}