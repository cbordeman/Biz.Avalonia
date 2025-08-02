using Avalonia;
using Avalonia.Controls;
using Biz.Shell.Services;
using Prism.Ioc;

namespace Biz.Shell.Views;

/// <summary>DashboardView.</summary>
public partial class DashboardView : UserControl
{
    public DashboardView()
    {
        InitializeComponent();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        // Initialize the WindowNotificationManager with the "TopLevel". Previously (v0.10), MainWindow
        var notifyService = ContainerLocator.Current.Resolve<INotificationService>();
#pragma warning disable CS8604 // Possible null reference argument.
        notifyService.SetHostWindow(TopLevel.GetTopLevel(this));
#pragma warning restore CS8604 // Possible null reference argument.
    }
}