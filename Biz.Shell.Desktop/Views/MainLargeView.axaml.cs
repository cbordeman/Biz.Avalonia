using Biz.Shell.Desktop.ViewModels;
using Biz.Shared.Views;

namespace Biz.Shell.Desktop.Views;

public partial class MainLargeView : FormFactorAwareUserControl<MainLargeViewModel>
{
    public MainLargeView()
    {
        InitializeComponent();
    }
}