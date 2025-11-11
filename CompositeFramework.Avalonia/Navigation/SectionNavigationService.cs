using System.Collections.Concurrent;
using System.Windows.Input;
using CompositeFramework.Core.Extensions;
using CompositeFramework.Modules;
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
    readonly IModuleManager moduleManager;
    ContentControl? ContentControl { get; set; }

    public string? SectionName { get; set; }

    readonly Stack<LocationWithViewInstance> history = [];
    public ILocation[] History =>
        history
            .Where(l => l.Location.AddToHistory)
            .Select(x => x.Location)
            .ToArray();

    readonly List<LocationWithViewInstance> forwardHistory = [];
    public ILocation[] ForwardHistory =>
        forwardHistory
            .Where(l => l.Location.AddToHistory)
            .Select(x => x.Location)
            .ToArray();

    public ICommand NavigateForwardCommand { get; }
    public ICommand NavigateBackCommand { get; }

    readonly ConcurrentDictionary<string, ViewModelViewBinding> registrations = new();
    public IReadOnlyDictionary<string, ViewModelViewBinding> Registrations =>
        registrations;

    public SectionNavigationService(IModuleManager moduleManager)
    {
        this.moduleManager = moduleManager;
    }

    public void Initialize(string sectionName)
    {
        if (!SectionManager.SectionNameRegistrations.TryGetValue(
                sectionName,
                out var cc))
            throw new NavigationSectionNameNotFoundException(
                sectionName,
                this);
        SectionName = sectionName;
        ContentControl = cc;
    }

    public async Task<NavigationResult> Refresh(
        string? alternativeModulename,
        string alternateLocationName,
        bool createNew = true)
    {
        if (SectionName == null || ContentControl == null)
            throw new NavigationSectionNameNotSetException(
                ContentControl, SectionName);

        if (current == null)
            return await NavigateToAsync(
                alternativeModulename,
                alternateLocationName);
        
        try
        {
            var vmBinding = registrations
                [current.Location.LocationName];
            var view = current.ViewInstance;

            if (createNew || view == null)
                view = (Control)Locator.Current
                    .Resolve(vmBinding.ViewType);

            if (createNew)
            {
                // Recreate a new ViewModel.
                var name = current.Location.LocationName;
                current.Location = (ILocation)Locator.Current.Resolve(
                    current.Location.GetType());
                current.Location.LocationName = name;
            }
            view.DataContext = current.Location;
            ContentControl.Content = view;

            SectionManager.ChangeSlideAnimation(SectionName!,
                NavigationDirection.Refresh);

            if (current.Location.KeepViewAlive)
                current.ViewInstance = view;

            await current.Location.OnNavigatedToAsync(
                new NavigationContext()
                {
                    Direction = NavigationDirection.Refresh,
                    Location = current.Location
                }
            );
            return NavigationResult.Success;
        }
        catch (Exception e)
        {
            try
            {
                await Navigated.PublishSequentiallyAsync(
                    new NavigatedEventArgs(
                        NavigationResult.Error,
                        current.Location.LocationName,
                        e,
                        new NavigationContext()
                        {
                            Location = current.Location,
                            Direction = NavigationDirection.Refresh
                        }));
            }
            catch (Exception exception)
            {
                // Eat
            }
            return NavigationResult.Error;
        }
    }

    public AsyncEvent<NavigatedEventArgs> Navigated { get; } = new();

    public async Task<NavigationResult> NavigateToAsync(
        string? moduleName, string location,
        params NavParam[] parameters)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(SectionName);
        ArgumentChecker.ThrowIfNullOrWhiteSpace(location);

        if (SectionName == null || ContentControl == null)
            throw new NavigationSectionNameNotSetException(ContentControl, SectionName);

        if (moduleName != null)
            await moduleManager.LoadModuleAsync(moduleName);

        var newNavCtx = new NavigationContext()
        {
            Direction = NavigationDirection.Forward,
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
                    $"used as a navigation location",
                    null);
            }
            vmLocation.NavigationParameters = parameters;
            vmLocation.LocationName = location;
            newNavCtx.Location = vmLocation;

            // Tell current page we're navigating.
            if (current != null)
            {
                var ok = await current.Location
                    .CanNavigateForwardAsync(newNavCtx);
                if (!ok)
                {
                    try
                    {
                        await Navigated.PublishSequentiallyAsync(
                            new NavigatedEventArgs(
                                NavigationResult.Cancelled,
                                location,
                                null,
                                newNavCtx));
                    }
                    catch (Exception ex)
                    {
                        // Eat
                    }
                    return NavigationResult.Cancelled;
                }
            }
            
            // Create view now since it is used in events.
            var v = Locator.Current.Resolve(vmBinding.ViewType);
            if (v is not Control view)
            {
                throw new TypeConstraintNotMetException(
                    Locator.Current,
                    vmBinding.ViewType,
                    $"View must derive from {nameof(Control)} to be " +
                    $"used as a navigation view.",
                    null);
            }
            view.DataContext = vmLocation;
            
            // Adjust history
            ClearForwardHistory();

            // Push current location onto history.
            if (current != null)
            {
                history.Push(new LocationWithViewInstance(
                    current.Location,
                    current.ViewType,
                    current.Location.KeepViewAlive ? 
                        current.ViewInstance : null));
            }
            
            // Set new current location.
            // Note that we hold a referene to view
            // here, in current.ViewInstance.  This
            // is not necessarily stored for history
            // items.
            current = new LocationWithViewInstance(
                vmLocation,
                vmBinding.ViewType,
                view);
            
            // Tell current page we're navigating.
            // At this point, history and current location
            // are already adjusted, so handler can initiate
            // navigation itself without corrupting state.
            await current.Location.OnNavigatingFromAsync(newNavCtx);
            
            // Swap the view.
            
            // Adjust animation.
            SectionManager.ChangeSlideAnimation(SectionName!,
                NavigationDirection.Forward);
            
            if (ContentControl is not { } contentControl)
            {
                throw new TypeConstraintNotMetException(
                    Locator.Current,
                    ContentControl.GetType(),
                    $"Section target must be a {nameof(global::Avalonia.Controls.ContentControl)}.",
                    null);
            }
            contentControl.Content = view;

            // Inform old and new current page.  Must do this
            // after adjusting history because these handlers
            // may want to navigate to another page.
            await vmLocation.OnNavigatedFromAsync(newNavCtx);
            await current.Location.OnNavigatedToAsync(newNavCtx);
        }
        catch (Exception e)
        {
            try
            {
                await Navigated.PublishSequentiallyAsync(
                    new NavigatedEventArgs(
                        NavigationResult.Error,
                        location,
                        e,
                        newNavCtx));
            }
            catch (Exception exception)
            {
                // Eat
            }
            return NavigationResult.Error;
        }

        try
        {
            await Navigated.PublishSequentiallyAsync(
                new NavigatedEventArgs(
                    NavigationResult.Success,
                    location,
                    null,
                    newNavCtx));
        }
        catch (Exception ex)
        {
            // Eat
        }
        return NavigationResult.Success;
    }

    public async Task<NavigationResult> GoBackAsync()
    {
        if (!history.Any(l => l.Location.AddToHistory))
            throw new InvalidOperationException(
                "Cannot go back.  History is empty.");
        if (current == null)
            throw new Exception("Current is null while history exists.");
        
        if (ContentControl == null)
            throw new NavigationSectionNameNotSetException();

        // Adjust history and find location to go
        // back to.
        LocationWithViewInstance? targetLocation = null;
        while (history.Count > 0)
        {
            var h = history.Pop();
            forwardHistory.Insert(0, h);
            if (h.Location.AddToHistory)
            {
                targetLocation = h;
                break;
            }
        }
        if (targetLocation == null)
            throw new InvalidOperationException(
                "No back history.  Shouldn't get here.");
        
        
        var oldCurrent = current;
        current = targetLocation;
            
        var newNavCtx = new NavigationContext()
        {
            Direction = NavigationDirection.Backward,
            Location = targetLocation.Location
        };

        try
        {
            // Tell current page we're navigating.
            // At this point, history and current location
            // are already adjusted, so handler can initiate
            // navigation itself without corrupting state.
            await oldCurrent.Location.OnNavigatingFromAsync(newNavCtx);

            // Swap the view.

            // Going back, use reverse animation.  Only works if user
            // used ReversibleTransitioningContentControl.
            SectionManager.ChangeSlideAnimation(SectionName!,
                NavigationDirection.Backward);

            // If didn't save view instance, create new one.
            var view = targetLocation.ViewInstance ??
                       Locator.Current.Resolve(targetLocation.ViewType);
            ContentControl.Content = view;
            
            // After navigation
            await oldCurrent.Location.OnNavigatedFromAsync(newNavCtx);
            await targetLocation.Location.OnNavigatedToAsync(newNavCtx);
        }
        catch (Exception e)
        {
            await Navigated.PublishSequentiallyAsync(
                new NavigatedEventArgs(
                    NavigationResult.Error,
                    current.Location.LocationName,
                    e,
                    newNavCtx));
            return NavigationResult.Error;
        }

        try
        {
            await Navigated.PublishSequentiallyAsync(
                new NavigatedEventArgs(
                    NavigationResult.Success,
                    current.Location.LocationName,
                    null,
                    newNavCtx));
        }
        catch (Exception ex)
        {
            // Eat
        }

        return NavigationResult.Success;
    }

    public Task<NavigationResult> GoForwardAsync(ILocation? toLocation = null)
    {
        throw new NotImplementedException();
    }

    public void ClearHistory()
    {
        ClearForwardHistory();

        // Clear normal history, except top of the stack,
        // which is the current location.
        if (history.Count >= 1)
        {
            var cur = history.Peek();
            history.Clear();
            history.Push(cur);
        }
    }

    public void ClearForwardHistory() =>
        forwardHistory.Clear();

    LocationWithViewInstance? current;

    public ILocation? CurrentLocation => current?.Location;

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
