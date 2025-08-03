using System;
using Avalonia.Controls.Platform;

namespace Biz.Shell.Views;

public partial class MainSmallView : UserControl
{
    IFormFactorService? formFactorService;

    public MainSmallView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        if (this.formFactorService == null && DataContext is IFormFactorService service)
            this.formFactorService = service;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        var topLevel = TopLevel.GetTopLevel((Visual)this.Parent!);
        if (topLevel == null) return;

        // Use topLevel.Bounds.Size here for initial layout decisions

        topLevel.SizeChanged -= TopLevel_SizeChanged;
        topLevel.SizeChanged += TopLevel_SizeChanged;
        // if (topLevel.InsetsManager != null)
        // {
        //     topLevel.InsetsManager.SafeAreaChanged += InsetsManager_SafeAreaChanged;
        // }
    }

    private void TopLevel_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        if (e.WidthChanged && this.formFactorService != null)
            this.formFactorService.NotifyWidthChanged(e.NewSize.Width);
    }

    private void InsetsManager_SafeAreaChanged(object? sender, SafeAreaChangedArgs e)
    {
        // React to safe area inset changes (e.g., notch, status bar)
    }
}