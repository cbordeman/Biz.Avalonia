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
    public ILocation[] History =>
        history
            // Exclude last item
            .Except(history.Reverse().Take(1))
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
                sectionName,
                out var cc))
            throw new NavigationSectionNameNotFoundException(
                sectionName,
                this);
        SectionName = sectionName;
        ContentControl = cc;
    }

    public async Task<NavigationResult> Refresh(string alternateLocationName)
    {
        if (history.TryPeek(out var location))
        {
            try
            {
                location.Location = (ILocation)Locator.Current.Resolve(
                    location.Location.GetType());
                await location.Location.OnNavigatedToAsync(
                    new NavigationContext()
                    {
                        Direction = NavitationDirection.Refresh,
                        Location = location.Location
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
                            location.Location.LocationName,
                            e,
                            new NavigationContext()
                            {
                                Location = location.Location,
                                Direction = NavitationDirection.Refresh
                            }));
                }
                catch (Exception exception)
                {
                    // Eat
                }
                return NavigationResult.Error;
            }
        }
        else
            return await NavigateToAsync(alternateLocationName);
    }

    public AsyncEvent<NavigatedEventArgs> Navigated { get; } = new();

    public async Task<NavigationResult> NavigateToAsync(string location,
        params NavParam[] parameters)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(SectionName);
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
                    $"used as a navigation location",
                    null);
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

            // Swap the view.
            SectionManager.ChangeSlideDirection(SectionName!, false);
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
            if (ContentControl is not { } contentControl)
            {
                throw new TypeConstraintNotMetException(
                    Locator.Current,
                    ContentControl.GetType(),
                    $"Section target must be a {nameof(global::Avalonia.Controls.ContentControl)}.",
                    null);
            }
            contentControl.Content = view;

            // Inform new page.
            if (currentLocation != null)
                await currentLocation.OnNavigatedFromAsync(newNavCtx);
            await vmLocation.OnNavigatedToAsync(newNavCtx);

            // Adjust history last so no side effects from user
            // exceptions.
            ClearForwardHistory();
            history.Push(new LocationWithViewInstance(
                vmLocation,
                vmBinding.ViewType,
                vmLocation.KeepViewAlive ? view : null));
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
        if (history.Count(l => l.Location.AddToHistory) < 2)
            throw new InvalidOperationException(
                "Cannot go back.  History is empty.");

        if (ContentControl == null)
            throw new NavigationSectionNameNotSetException();

        // Find location to go back to.  Remove from top
        // of history and add to forwardHistory.  Only save
        // locations where AddToHistory is true.
        LocationWithViewInstance? targetLocation;
        ILocation? currentLocation;
        int curIndex = history.Count - 1;
        var h = history.ToArray();
        while (true)
        {
            var top = h[curIndex];
            if (top.Location.AddToHistory)
            {
                currentLocation = top.Location;
                targetLocation = h[curIndex - 1];
                break;
            }
            else
                curIndex--;
        }

        var newNavCtx = new NavigationContext()
        {
            Direction = NavitationDirection.Backward,
            Location = targetLocation.Location
        };

        try
        {
            // Tell current page we're navigating.
            await currentLocation.OnNavigatingFromAsync(newNavCtx);

            // Swap the view.

            // Going back, use reverse animation.  Only works if user
            // used ReversibleTransitioningContentControl.
            SectionManager.ChangeSlideDirection(SectionName!, true);

            // If didn't save view instance, create new one.
            var view = targetLocation.ViewInstance ??
                       Locator.Current.Resolve(targetLocation.ViewType);
            ContentControl.Content = view;

            // After navigation
            await currentLocation.OnNavigatedFromAsync(newNavCtx);
            await targetLocation.Location.OnNavigatedToAsync(newNavCtx);
        }
        catch (Exception e)
        {
            await Navigated.PublishSequentiallyAsync(
                new NavigatedEventArgs(
                    NavigationResult.Error,
                    currentLocation.LocationName,
                    e,
                    newNavCtx));
            return NavigationResult.Error;
        }

        try
        {
            await Navigated.PublishSequentiallyAsync(
                new NavigatedEventArgs(
                    NavigationResult.Success,
                    currentLocation.LocationName,
                    null,
                    newNavCtx));
        }
        catch (Exception ex)
        {
            // Eat
        }

        // Adjust history last so that errors don't corrupt history.
        while (true)
        {
            var top = history.Peek();
            if (top.Location.AddToHistory)
            {
                // Move location from history to forwardHistory.
                var popped = history.Pop();
                forwardHistory.Insert(0, popped);
                break;
            }
            else
                history.Pop();
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
        if (history.Count > 1)
        {
            var cur = history.Peek();
            history.Clear();
            history.Push(cur);
        }
    }

    public void ClearForwardHistory() =>
        forwardHistory.Clear();

    public ILocation? CurrentLocation
    {
        get
        {
            if (history.Count == 0)
                return null;
            return history.Peek().Location;
        }
    }

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
