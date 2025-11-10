using Avalonia.Animation;

namespace CompositeFramework.Avalonia.Controls;

/// <summary>
/// A TransitioningContentControl with a Reverse property.
/// </summary>
public class ReversibleTransitioningContentControl : TransitioningContentControl
{
    protected override Type StyleKeyOverride => typeof(TransitioningContentControl);
    
    public static readonly StyledProperty<NavigationDirection> ReverseProperty =
        AvaloniaProperty.Register<ReversibleTransitioningContentControl, NavigationDirection>(nameof(Direction));

    IPageTransition? innerTransition;
    bool ignoreTransitionChanging;
    
    public ReversibleTransitioningContentControl()
    {
        innerTransition = PageTransition;
        this.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object? sender, 
        AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property == PageTransitionProperty)
        {
            // To prevent loop while replacing PageTransition
            // with a wrapper.
            if (ignoreTransitionChanging)
                return;
            
            // Need to save the initial transition.
            if (innerTransition == null)
                innerTransition = PageTransition;
        
            if (PageTransition == null)
            {
                innerTransition = null;
                return;
            }
        
            innerTransition = e.NewValue as IPageTransition;
            UpdateTransition();
        }
        else if (e.Property == ReverseProperty)
        {
            if (PageTransition is DelegatedPageTransition dt)
                dt.Direction = Direction;
            else
            {
                innerTransition = PageTransition;
                UpdateTransition();
                if (PageTransition is DelegatedPageTransition dt2)
                    dt2.Direction = Direction;
            }
        }
    }

    private void UpdateTransition()
    {
        if (innerTransition == null)
            throw new InvalidOperationException("PageTransition cannot be null.");
        try
        {
            ignoreTransitionChanging = true;
            PageTransition =
                new DelegatedPageTransition(innerTransition!, Direction);
        }
        finally
        {
            ignoreTransitionChanging = false;
        }
    }

    public NavigationDirection Direction
    {
        get => GetValue(ReverseProperty);
        set => SetValue(ReverseProperty, value);
    }
}

/// <summary>
/// IPageTransition wrapper that applies forceForward parameter.
/// </summary>
public class DelegatedPageTransition : IPageTransition
{
    public override string ToString() => nameof(DelegatedPageTransition);
    
    // ReSharper disable once MemberCanBePrivate.Global
    public IPageTransition Inner { get; }
    public NavigationDirection Direction { get; set; }
    
    /// <summary>
    /// IPageTransition wrapper that applies direction parameter.
    /// </summary>
    public DelegatedPageTransition(IPageTransition inner,
        NavigationDirection direction)
    {
        this.Inner = inner;
        this.Direction = direction;
    }
    
    public Task Start(Visual? from, Visual? to, bool forward, 
        CancellationToken ct)
    {
        return Direction switch
        {
            NavigationDirection.Forward => Inner.Start(from, to, true, ct),
            NavigationDirection.Backward => Inner.Start(from, to, false, ct),
            NavigationDirection.Refresh => Task.CompletedTask,
            _ => throw new ArgumentOutOfRangeException(nameof(Direction))
        };
    }
}