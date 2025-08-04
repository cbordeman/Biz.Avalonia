namespace Biz.Core.ViewModels;

public abstract class NavigationAwareViewModelBase : FormFactorAwareViewModel, INavigationAware,
    IConfirmNavigationRequest
{
    protected readonly IRegionManager? RegionManager;
    protected IRegionNavigationService RegionNavigationService = null!;

    #region Title
    string title = string.Empty;
    public string Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }
    #endregion Title

    protected NavigationAwareViewModelBase(IContainer container) 
        : base(container)
    {
        this.RegionManager = container.Resolve<IRegionManager>();
    }
    
    /// <summary>
    ///   Called to determine if this instance can handle the navigation request.
    /// </summary>
    /// <param name="navigationContext">The navigation context.</param>
    /// <returns><see langword="true"/> if this instance accepts the navigation request; otherwise, <see langword="false"/>.</returns>
    public virtual bool IsNavigationTarget(NavigationContext navigationContext)
    {
        // Auto-allow navigation
        return OnNavigatingTo(navigationContext);
    }

    /// <summary>Called when the implementer is being navigated away from.</summary>
    /// <param name="navigationContext">The navigation context.</param>
    public virtual void OnNavigatedFrom(NavigationContext navigationContext)
    { }

    /// <summary>
    /// Determines whether this instance accepts being navigated away from.
    /// </summary>
    /// <param name="navigationContext">The navigation context.</param>
    /// <param name="continuationCallback">The callback to indicate when navigation can proceed.</param>
    /// <remarks>
    /// Implementors of this method do not need to invoke the callback before this method is completed,
    /// but they must ensure the callback is eventually invoked.
    /// </remarks>
    public virtual void ConfirmNavigationRequest(NavigationContext navigationContext,
        Action<bool> continuationCallback)
    {
        continuationCallback(true);
    }

    /// <summary>Called when the implementer has been navigated to.</summary>
    /// <param name="navigationContext">The navigation context.</param>
    public virtual void OnNavigatedTo(NavigationContext navigationContext)
    {
        RegionNavigationService = navigationContext.NavigationService;
    }

    /// <summary>Navigation validation checker.</summary>
    /// <remarks>Override for Prism 7.2's IsNavigationTarget.</remarks>
    /// <param name="navigationContext">The navigation context.</param>
    /// <returns><see langword="true"/> if this instance accepts the navigation request; otherwise, <see langword="false"/>.</returns>
    protected virtual bool OnNavigatingTo(NavigationContext navigationContext) 
        => true;
}