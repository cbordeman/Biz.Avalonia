using Biz.Shell.Desktop.ViewModels;
using Biz.Shell.ViewModels;
using Biz.Shell.Views;

namespace Biz.Shell.Desktop.Views;

public partial class MainLargeView : FormFactorAwareUserControl<MainLargeViewModel>
{
    public MainLargeView()
    {
        InitializeComponent();
    }
}