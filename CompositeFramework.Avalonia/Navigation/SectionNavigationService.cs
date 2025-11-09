using System.Collections.Concurrent;
using System.Windows.Input;
using CompositeFramework.Core.Extensions;
using Splat;

namespace CompositeFramework.Avalonia.Navigation;

/// <summary>
/// Implementation of ISectionNavigationService that navigates
/// within a single section.  Call Initialize(sectionName) before use.
/// In AXAML, set SectionManager.SectionName on a ContentControl (or
/// derived type).
/// </summary>
public class SectionNavigationService
    : ISectionNavigationService
{
    ContentControl? ContentControl { get; set; }

    public string? SectionName { get; set; }
    
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

    readonly ConcurrentDictionary<string, ViewModelViewBinding> registrations = new();
    public IReadOnlyDictionary<string, ViewModelViewBinding> Registrations =>
        registrations;
    
    public void Initialize(string sectionName)
    {
        if (!SectionManager.SectionNameRegistrations.TryGetValue(
                sectionName, out var cc))
            throw new NavigationSectionNameNotFoundException(
                sectionName, this);
        ContentControl = cc;        
    }
    
    public async Task Refresh(string alternateLocationName)
    {
        if (history.TryPeek(out var location))
        {
            location.Location =(ILocation) Locator.Current.Resolve(
                location.Location.GetType());
            await location.Location.OnNavigatedToAsync(
                new NavigationContext()
                {
                    Direction = NavitationDirection.Refresh,
                    Location = location.Location }
                );
        }
        else
        {
            await NavigateToAsync(alternateLocationName);
        }
    }

    public AsyncEvent<NavigatedEventArgs> Navigated { get; } = new();

    public async Task<NavigationResult> NavigateToAsync(string location, 
        params NavParam[] parameters)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(location);
        
        if (ContentControl == null)
            throw new NavigationSectionNameNotSetException();

        var newNavCtx = new NavigationContext()
        {
            Direction = NavitationDirection.Forward,
        };
            
        try
        {
            if (!registrations.TryGetValue(location, out var vmBinding))
                throw new Exception(
                    $"No navigation registration found for " +
                    $"location \"{location}\" in this instance of " +
                    $"IContextNavigationService.  Call RegisterForNavigation() " +
                    $"before navigating.");

            // Note that the locator could be returning a singleton.
            var vm = Locator.Current.Resolve(vmBinding.ViewModelType);
            if (vm is not ILocation vmLocation)
            {
                throw new TypeConstraintNotMetException(
                    Locator.Current,
                    vmBinding.ViewModelType,
                    $"Must implement {nameof(ILocation)} to be " +
                    $"used as a navigation location", null);
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
            if (v is not Control view)
            {
                throw new TypeConstraintNotMetException(
                    Locator.Current,
                    vmBinding.ViewType,
                        $"View must derive from {nameof(Control)} to be " +
                        $"used as a navigation view.", null);
            }
            view.DataContext = vmLocation;
            if (ContentControl is not { } contentControl)
            {
                throw new TypeConstraintNotMetException(
                    Locator.Current,
                    ContentControl.GetType(),
                    $"Section target must be a {nameof(global::Avalonia.Controls.ContentControl)}.",
                    null);
            }
            contentControl.Content = view;

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

    public Task<NavigationResult> GoBackAsync()
    {
        if (history.Count == 0)
            throw new InvalidOperationException("Cannot go back.  History is empty.");
        
        if (ContentControl == null)
            throw new NavigationSectionNameNotSetException();

        var newNavCtx = new NavigationContext()
        {
            Direction = NavitationDirection.Backward,
        };
            
        try
        {
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
            if (v is not Control view)
            {
                throw new TypeConstraintNotMetException(
                    Locator.Current,
                    vmBinding.ViewType,
                        $"View must derive from {nameof(Control)} to be " +
                        $"used as a navigation view.", null);
            }
            view.DataContext = vmLocation;
            if (ContentControl is not { } contentControl)
            {
                throw new TypeConstraintNotMetException(
                    Locator.Current,
                    ContentControl.GetType(),
                    $"Section target must be a {nameof(global::Avalonia.Controls.ContentControl)}.",
                    null);
            }
            contentControl.Content = view;

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
                new ViewModelViewBinding(typeof(TViewModel), typeof(TView))))
        {
            throw new DuplicateNavigationRegistrationException
                <TViewModel, TView>(locationName);
        }
    }
}