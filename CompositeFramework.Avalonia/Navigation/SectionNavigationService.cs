using System.Collections.Concurrent;
using System.Windows.Input;
using CompositeFramework.Core.Extensions;
using Splat;

namespace CompositeFramework.Avalonia.Navigation;

/// <summary>
/// Implementation of IContextNavigationService that navigates
/// within a single space.
/// </summary>
public class SectionNavigationService
    : IContextNavigationService
{
    public object? Context { get; set; }

    readonly Stack<LocationWithViewInstance> history = [];
    public IReadOnlyCollection<ILocation> History =>
        history
            .Where(l => l.Location.AddToHistory)
            .Select(x => x.Location)
            .ToArray();

    readonly List<LocationWithViewInstance> forwardHistory = [];
    public IReadOnlyCollection<ILocation> ForwardHistory =>
        forwardHistory
            .Where(l => l.Location.AddToHistory)
            .Select(x => x.Location)
            .ToArray();

    public ICommand NavigateForwardCommand { get; }
    public ICommand NavigateBackCommand { get; }

    readonly ConcurrentDictionary<string, ViewModelLocationBinding> registrations = new();
    public IReadOnlyDictionary<string, ViewModelLocationBinding> Registrations =>
        registrations;

    public AsyncEvent<NavigatedEventArgs> Navigated { get; } = new();

    public async Task<NavigationResult> NavigateToAsync(string location, 
        params NavParam[] parameters)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(location);
        
        if (Context == null)
            throw new NavigationContextNotSetException();

        var newNavCtx = new NavigationContext()
        {
            Direction = NavitationDirection.Forward,
        };
            
        try
        {
            var control = (ContentControl)Context!;
            if (!registrations.TryGetValue(location, out var vmBinding))
                throw new Exception(
                    $"No navigation registration found for " +
                    $"location \"{location}\" in this instance of " +
                    $"IContextNavigationService.  Call RegisterForNavigation() " +
                    $"before navigating.");

            // Not that the locator could be returning a singleton.
            var vm = Locator.Current.Resolve(vmBinding.ViewModelType);
            if (vm is not ILocation vmLocation)
            {
                throw new Exception($"Type {vmBinding.ViewModelType} must " +
                                    $"implement {nameof(ILocation)} to be " +
                                    $"used as a navigation location.");
            }
            vmLocation.NavigationParameters = parameters;
            vmLocation.LocationName = location;
            newNavCtx.Location = vmLocation;
            
            // Tell current page we're navigating.
            ILocation? currentLocation = null;
            if (history.Count > 0)
            {
                currentLocation = history.Peek().Location;
                var ok = await currentLocation
                    .OnNavigatingFromAsync(newNavCtx);
                if (!ok)
                {
                    await Navigated.PublishSequentiallyAsync(
                        new NavigatedEventArgs(
                            NavigationResult.Cancelled,
                            location,
                            null,
                            newNavCtx));
                    return NavigationResult.Cancelled;
                }
            }

            // Swap the view.
            var v = Locator.Current.Resolve(vmBinding.ViewType);
            if (vm is not Control view)
            {
                throw new Exception($"Type {vmBinding.ViewType} must " +
                                    $"derive from {nameof(Control)} to be " +
                                    $"used as a navigation view.");
            }
            view.DataContext = vmLocation;
            control.Content = view;

            // Adjust history
            ClearForwardHistory();
            history.Push(new LocationWithViewInstance(
                vmLocation,
                vmLocation.KeepViewAlive ? view : null));

            // After navigation
            if (currentLocation != null)
                await currentLocation.OnNavigatedFromAsync(newNavCtx);
            await vmLocation.OnNavigatedToAsync(newNavCtx);
        }
        catch (Exception e)
        {
            await Navigated.PublishSequentiallyAsync(
                new NavigatedEventArgs(
                    NavigationResult.Error,
                    location,
                    e, 
                    newNavCtx));
            return NavigationResult.Error;
        }
        
        await Navigated.PublishSequentiallyAsync(
            new NavigatedEventArgs(
                NavigationResult.Success,
                location,
                null,
                newNavCtx));
        return NavigationResult.Success;
    }

    public Task<NavigationResult> GoBackAsync(ILocation? toLocation = null)
    {
        throw new NotImplementedException();
    }
    
    public Task<NavigationResult> GoForwardAsync(ILocation? toLocation = null)
    {
        throw new NotImplementedException();
    }

    public void ClearHistory()
    {
        if (forwardHistory.Count > 1)
        {
            var cur = history.Peek();
            history.Clear();
            history.Push(cur);
        }
    }

    public void ClearForwardHistory() =>
        forwardHistory.Clear();

    public void RegisterForNavigation<TViewModel, TView>
        (string? locationName = null)
        where TViewModel : INotifyPropertyChanged, ILocation
    {
        locationName ??= typeof(TViewModel).FullName!;

        ArgumentChecker.ThrowIfNullOrWhiteSpace(locationName);
        
        if (!registrations.TryAdd(locationName,
                new ViewModelLocationBinding(typeof(TViewModel), typeof(TView))))
        {
            throw new DuplicateNavigationRegistrationException
                <TViewModel, TView>(locationName);
        }
    }
}