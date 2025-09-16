namespace CompositeFramework.Core.Navigation;

public enum NavigationResult
{
    Success = 0,
    
    /// <summary>
    /// If OperationCanceledException was thrown during
    /// IContextNavigationService.NavigateToAsync() or by
    /// the methods in INavigationAware.
    /// </summary>
    Cancelled = 1,
    
    /// <summary>
    /// Value when IAllowNavigation.CanNavigateToAsync
    /// returns false.
    /// </summary>
    NotAllowed = 2,
    
    /// <summary>
    /// When an exception other than OperationCancelledException
    /// is thrown during
    /// IContextNavigationService.NavigateToAsync().
    /// </summary>
    Error = 3
}