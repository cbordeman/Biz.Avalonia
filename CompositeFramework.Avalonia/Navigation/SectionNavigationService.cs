using System.Collections.Concurrent;
using System.ComponentModel;
using System.Windows.Input;
using Avalonia.Controls;
using CompositeFramework.Avalonia.Exceptions;

namespace CompositeFramework.Avalonia.Navigation;

/// <summary>
/// Implementation of IContextNavigationService that navigates
/// within a single space.
/// </summary>
public class SectionNavigationService : IContextNavigationService
{
    public object? Context { get; set; }
    
    readonly List<ILocation> history = [];
    public IReadOnlyCollection<ILocation> History => history.AsReadOnly();

    public int CurrentLocationIndex { get; private set; } = -1;
    public ICommand NavigateForwardCommand { get; }
    public ICommand NavigateBackCommand { get; }

    readonly ConcurrentDictionary<Type, Type> navigationBindings = new();
    public IReadOnlyDictionary<Type, Type> NavigationBindings => 
        navigationBindings.AsReadOnly<Type, Type>();

    public AsyncEvent<NavigatedEventArgs> Navigated { get; } = new();
    
    public Task<bool> NavigateToAsync(string location, params NavParam[] parameters)
    {
        if (Context == null)
            throw new NavigationContextNotSetException();
        
        ArgumentChecker.ThrowIfNullOrEmpty(location);
        
        return Task.FromResult(true);
    }
    
    public Task<bool> GoBackAsync(ILocation? toLocation = null)
    {
        return Task.FromResult(true);
    }
    
    public void ClearHistory()
    {
        history.Clear();
        CurrentLocationIndex = -1;
    }

    public void RegisterNavigation<TViewModel, TView>(string? locationName = null)
        where TViewModel : INotifyPropertyChanged where TView : ILocation
    {
        if (!navigationBindings.TryAdd(typeof(TViewModel), typeof(TView)))
            throw new DuplicateViewModelBindingException<TViewModel, TView>();
    }
}