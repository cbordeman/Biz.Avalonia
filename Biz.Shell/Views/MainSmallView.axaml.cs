using Avalonia.Controls.Platform;
using Biz.Shell.Core.Services;

namespace Biz.Shell.Views;

public partial class MainSmallView : TopLevelUserControl
{
    IFormFactorService? formFactorService;

    public MainSmallView()
    {
        InitializeComponent();
    }
}