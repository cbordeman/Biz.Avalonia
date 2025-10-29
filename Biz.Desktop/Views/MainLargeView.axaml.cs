using Biz.Desktop.ViewModels;
using Biz.Shared.Views;

namespace Biz.Desktop.Views;

public partial class MainLargeView : FormFactorAwareUserControl
    <MainLargeViewModel>
{
    public MainLargeView()
    {
        InitializeComponent();
    }
}