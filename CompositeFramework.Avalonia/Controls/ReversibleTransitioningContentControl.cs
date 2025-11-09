using Avalonia.Animation;

namespace CompositeFramework.Avalonia.Controls;

/// <summary>
/// A TransitioningContentControl with a SlideLeft property.
/// </summary>
public class ReversibleTransitioningContentControl : TransitioningContentControl
{
    public static readonly StyledProperty<bool> SlideLeftProperty =
        AvaloniaProperty.Register<ReversibleTransitioningContentControl, bool>(nameof(SlideLeft));

    IPageTransition? innerTransition;
    bool ignoreTransitionChanging;
    
    public ReversibleTransitioningContentControl()
    {
        innerTransition = PageTransition;

        this.PropertyChanged += OnPropertyChanged;

        UpdateTransition();
    }

    private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        // Need to save the initial transition.
        innerTransition ??= PageTransition;

        if (PageTransition == null)
        {
            innerTransition = PageTransition;
            return;
        }
        
        // To prevent infinite loop while replacing PageTransition
        // with a wrapper.
        if (ignoreTransitionChanging)
        {
            ignoreTransitionChanging = false;
            return;
        }

        if (e.Property == PageTransitionProperty)
        {
            innerTransition = PageTransition;
            UpdateTransition();
        }
        else if (e.Property == SlideLeftProperty)
            UpdateTransition();
    }

    private void UpdateTransition()
    {
        if (innerTransition == null)
            throw new InvalidOperationException("PageTransition cannot be null.");
        ignoreTransitionChanging = true;
        PageTransition = 
            new DelegatedPageTransition(innerTransition!, SlideLeft);
    }

    public bool SlideLeft
    {
        get => GetValue(SlideLeftProperty);
        set => SetValue(SlideLeftProperty, value);
    }
}

// public class SlideLeftRightTransition : IPageTransition
// {
//     private readonly TimeSpan duration;
//
//     public SlideLeftRightTransition(TimeSpan duration)
//     {
//         this.duration = duration;
//     }
//
//     public async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
//     {
//         if (cancellationToken.IsCancellationRequested)
//             return;
//
//         var parent = GetVisualParent(from, to);
//         double width = parent.Bounds.Width;
//
//         if (from != null)
//         {
//             if (!(from.RenderTransform is TranslateTransform))
//             {
//                 from.RenderTransform = new TranslateTransform();
//             }
//             from.IsVisible = true;
//         }
//
//         if (to != null)
//         {
//             if (!(to.RenderTransform is TranslateTransform))
//             {
//                 to.RenderTransform = new TranslateTransform();
//             }
//             to.IsVisible = true;
//             ((TranslateTransform)to.RenderTransform).X = forward ? width : -width;
//         }
//
//         var animationOut = new Animation
//         {
//             Duration = duration,
//             FillMode = FillMode.Forward,
//             Children =
//             {
//                 new KeyFrame { Cue = new Cue(0), Setters = { new Setter(TranslateTransform.XProperty, 0d) } },
//                 new KeyFrame { Cue = new Cue(1), Setters = { new Setter(TranslateTransform.XProperty, forward ? -width : width) } }
//             }
//         };
//
//         var animationIn = new Animation
//         {
//             Duration = duration,
//             FillMode = FillMode.Forward,
//             Children =
//             {
//                 new KeyFrame { Cue = new Cue(0), Setters = { new Setter(TranslateTransform.XProperty, forward ? width : -width) } },
//                 new KeyFrame { Cue = new Cue(1), Setters = { new Setter(TranslateTransform.XProperty, 0d) } }
//             }
//         };
//
//         var taskOut = from != null ? animationOut.RunAsync((TranslateTransform)from.RenderTransform!, cancellationToken) : Task.CompletedTask;
//         var taskIn = to != null ? animationIn.RunAsync((TranslateTransform)to.RenderTransform!, cancellationToken) : Task.CompletedTask;
//
//         await Task.WhenAll(taskOut, taskIn);
//
//         if (from != null && !cancellationToken.IsCancellationRequested)
//         {
//             from.IsVisible = false;
//             ((TranslateTransform)from.RenderTransform!).X = 0;
//         }
//
//         if (to != null)
//             ((TranslateTransform)to.RenderTransform!).X = 0;
//     }
//
//     private static Visual GetVisualParent(Visual? from, Visual? to)
//     {
//         var p1 = (from ?? to)?.GetVisualParent();
//         var p2 = (to ?? from)?.GetVisualParent();
//
//         if (p1 != p2)
//             throw new ArgumentException("Transitions must share the same visual parent.");
//
//         return p1 ?? throw new InvalidOperationException("Unable to determine visual parent.");
//     }
// }

/// <summary>
/// IPageTransition wrapper that applies forceForward parameter.
/// </summary>
/// <param name="inner"></param>
/// <param name="forceForward"></param>
public class DelegatedPageTransition(IPageTransition inner,
    bool forceForward) : IPageTransition
{

    public Task Start(Visual? from, Visual? to, bool forward1, 
        CancellationToken cancellationToken) =>
        inner.Start(from, to, forceForward, cancellationToken);
}