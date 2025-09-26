using System.Collections.Concurrent;
using System.ComponentModel;
using System.Windows.Input;
using CompositeFramework.Avalonia.Exceptions;

namespace CompositeFramework.Avalonia.Navigation;

/// <summary>
/// Implementation of IContextNavigationService that navigates
/// within a single space.
/// </summary>
public class SectionNavigationService 
    : IContextNavigationService
{
    public object? Context { get; set; }

    readonly List<ILocation> history = [];
    public IReadOnlyCollection<ILocation> History => history.AsReadOnly();

    public int CurrentLocationIndex { get; private set; } = -1;
    public ICommand NavigateForwardCommand { get; }
    public ICommand NavigateBackCommand { get; }

    readonly ConcurrentDictionary<string, ViewModelViewBinding> registrations = new();
    public IReadOnlyDictionary<string, ViewModelViewBinding> Registrations =>
        registrations.AsReadOnly();

    public AsyncEvent<NavigatedEventArgs> Navigated { get; } = new();

    public Task<bool> NavigateToAsync(string location, params NavParam[] parameters)
    {
        if (Context == null)
            throw new NavigationContextNotSetException();

        ArgumentChecker.ThrowIfNullOrWhiteSpace(location);

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

    public void RegisterForNavigation<TViewModel, TView>
        (string? locationName = null)
        where TViewModel : INotifyPropertyChanged
        where TView : ILocation
    {
        locationName ??= typeof(TViewModel).FullName!;
        
        ArgumentChecker.ThrowIfNullOrWhiteSpace(locationName);

        if (!registrations.TryAdd(locationName,
                new ViewModelViewBinding(typeof(TViewModel), typeof(TView))))
        {
            throw new DuplicateNavigationRegistrationException
                <TViewModel, TView>(locationName);
        }
    }
}