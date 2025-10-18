// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable UnusedMember.Local

namespace CompositeFramework.Core.Navigation;

public interface ILocation
{
    Task OnNavigatedToAsync(NavigationContext context);
    Task<bool> OnNavigatingFromAsync(NavigationContext newContext); 
    Task OnNavigatedFromAsync(NavigationContext navigationContext);
    
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