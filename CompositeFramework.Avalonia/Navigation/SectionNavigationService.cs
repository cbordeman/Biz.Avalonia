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
            // Exclude last item, which is the current location.
            .Except(history.Reverse().Take(1))
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
        if (ContentControl == null)
            throw new NavigationSectionNameNotSetException();

        if (history.TryPeek(out var location))
        {
            try
            {
                var vmBinding = registrations
                    [location.Location.LocationName];
                var view = location.ViewInstance;

                if (createNew || view == null)
                    view = (Control)Locator.Current
                        .Resolve(vmBinding.ViewType);

                if (createNew)
                {
                    // Recreate a new ViewModel.
                    var name = location.Location.LocationName;
                    location.Location = (ILocation)Locator.Current.Resolve(
                        location.Location.GetType());
                    location.Location.LocationName = name;
                }
                view.DataContext = location.Location;
                ContentControl.Content = view;

                SectionManager.ChangeSlideAnimation(SectionName!,
                    NavigationDirection.Refresh);

                if (location.Location.KeepViewAlive)
                    location.ViewInstance = view;

                await location.Location.OnNavigatedToAsync(
                    new NavigationContext()
                    {
                        Direction = NavigationDirection.Refresh,
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
        else
            return await NavigateToAsync(
                alternativeModulename,
                alternateLocationName);
    }

    public AsyncEvent<NavigatedEventArgs> Navigated { get; } = new();

    public async Task<NavigationResult> NavigateToAsync(
        string? moduleName, string location,
        params NavParam[] parameters)
    {
        ArgumentChecker.ThrowIfNullOrWhiteSpace(SectionName);
        ArgumentChecker.ThrowIfNullOrWhiteSpace(location);

        if (ContentControl == null)
            throw new NavigationSectionNameNotSetException();

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
            SectionManager.ChangeSlideAnimation(SectionName!,
                NavigationDirection.Forward);
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
            Direction = NavigationDirection.Backward,
            Location = targetLocation.Location
        };

        try
        {
            // Tell current page we're navigating.
            await currentLocation.OnNavigatingFromAsync(newNavCtx);

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
