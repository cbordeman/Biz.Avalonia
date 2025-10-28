using Biz.Shared.Services;
using IFormFactorService = Biz.Shared.Services.IFormFactorService;
using Services_IFormFactorService = Biz.Shared.Services.IFormFactorService;

namespace Biz.Shared.Views;

public class FormFactorAwareUserControl<TViewModel> 
    : UserControlEx<TViewModel>, IDisposable 
    where TViewModel : class
{
    Services_IFormFactorService? formFactorService;
    TopLevel? topLevel;
    
    protected override void OnDataContextChanged(EventArgs e)
    {
        if (formFactorService == null && DataContext is Services_IFormFactorService service)
            formFactorService = service;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        topLevel = TopLevel.GetTopLevel((Visual)Parent!);
        if (topLevel == null) return;

        // Use topLevel.Bounds.Size here for initial layout decisions

        topLevel.SizeChanged -= TopLevel_SizeChanged;
        topLevel.SizeChanged += TopLevel_SizeChanged;
        
        // if (topLevel.InsetsManager != null)
        // {
        //     topLevel.InsetsManager.SafeAreaChanged += InsetsManager_SafeAreaChanged;
        // }
        
        // Initialize the WindowNotificationManager with the "TopLevel". Previously (v0.10), MainWindow
        var notifyService = Locator.Current.GetService<INotificationService>();
        notifyService?.SetHostWindow(TopLevel.GetTopLevel(this)!);
    }

    void TopLevel_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged && formFactorService != null)
            formFactorService.NotifyWidthChanged(e.NewSize.Width);
    }

    // private void InsetsManager_SafeAreaChanged(object? sender, SafeAreaChangedArgs e)
    // {
    //     // React to safe area inset changes (e.g., notch, status bar)
    // }

    public void Dispose()
    {
        if (topLevel != null)
        {
            topLevel.SizeChanged -= TopLevel_SizeChanged;
            topLevel = null;
        }
        GC.SuppressFinalize(this);
    }
}