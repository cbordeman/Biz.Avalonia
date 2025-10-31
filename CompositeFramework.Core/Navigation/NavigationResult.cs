namespace CompositeFramework.Core.Navigation;

public enum NavigationResult
{
    Success,
    
    /// <summary>
    /// Value when ILocation.OnNavigatingFromAsync()
    /// returned false.
    /// returns false.
    /// </summary>
    Cancelled,
    
    /// <summary>
    /// When an exception other than OperationCancelledException
    /// is thrown during
    /// IContextNavigationService.NavigateToAsync().
    /// </summary>
    Error
}