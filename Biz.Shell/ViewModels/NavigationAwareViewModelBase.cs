using System.Reflection;

namespace Biz.Shell.ViewModels;

public abstract class NavigationAwareViewModelBase()
    : FormFactorAwareViewModel, ILocation
    
{
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
    /// <param name="ctx">The navigation context.</param>
    public virtual Task OnNavigatedToAsync(NavigationContext ctx)
    {
        if (ctx.Parameters.Count == 0)
            return Task.CompletedTask;
        
        // Public non-static properties with a setter. 
        var properties = this.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod != null && p.SetMethod.IsPublic);
        properties.ShouldNotBeNull();
        foreach (var p in ctx.Parameters)
        {
            var foundProp = properties.FirstOrDefault(prop => prop.Name == p.Key);
            if (foundProp == null)
                throw new InvalidOperationException($"There is no property on {this.GetType().FullName} named \"{p.Key}\".");
            foundProp.SetValue(this, p.Value);
        }
        
        return Task.CompletedTask;
    }
    
    // public Task NavigateToAsync(string area, INavigationParameters? parameters = null)
    // {
    //     if (NavigationService != null)
    //     {
    //         var taskCompletionSource = new TaskCompletionSource<bool>();
    //         NavigationService!.RequestNavigate(
    //             new Uri(area),
    //             (NavigationResult args) =>
    //             {
    //                 if (args.Cancelled)
    //                     taskCompletionSource.SetCanceled();
    //                 else if (args.Exception != null)
    //                     taskCompletionSource.SetException(args.Exception);
    //                 else if (args.Success)
    //                     taskCompletionSource.SetResult(true);
    //             },
    //             parameters);
    //         return taskCompletionSource.Task;
    //     }
    //     return Task.FromException(new InvalidOperationException("Initialize() has not been called."));
    // }
    
    /// <summary>Navigation validation checker.</summary>
    /// <remarks>Override for Prism 7.2's IsNavigationTarget.</remarks>
    /// <param name="navigationContext">The navigation context.</param>
    /// <returns><see langword="true"/> if this instance accepts the navigation request; otherwise, <see langword="false"/>.</returns>
    
    protected virtual bool OnNavigatingTo(NavigationContext navigationContext) 
        => true;

    public virtual bool PersistInHistory() => true;
    
    public abstract string Area { get; }
}