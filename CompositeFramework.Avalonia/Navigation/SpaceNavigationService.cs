using System.Collections.Concurrent;
using System.ComponentModel;
using System.Windows.Input;

namespace CompositeFramework.Avalonia.Navigation;

/// <summary>
/// Implementation of IContextNavigationService that navigates
/// within a particular space.
/// </summary>
public class SpaceNavigationService : IContextNavigationService
{
    readonly List<ILocation> history = [];
    public IReadOnlyCollection<ILocation> History => history.AsReadOnly();
    
    public int? CurrentLocationIndex { get; }
    public ICommand NavigateForwardCommand { get; }
    public ICommand NavigateBackCommand { get; }

    readonly ConcurrentDictionary<Type, Type> bindings = new();
    public IReadOnlyDictionary<Type, Type> Bindings => bindings.AsReadOnly();

    public AsyncEvent<NavigatedEventArgs> Navigated { get; } = new();
    
    public Task<bool> NavigateToAsync(string location, params NavParam[] parameters)
    {
        return Task.FromResult(true);
    }
    
    public Task<bool> GoBackAsync(ILocation? toLocation = null)
    {
        return Task.FromResult(true);
    }
    
    public void ClearHistory()
    {
        
    }

    public void BindViewModelAndView<TViewModel, TView>(string? locationName = null)
        where TViewModel : INotifyPropertyChanged where TView : ILocation
    {
        
    }
}
