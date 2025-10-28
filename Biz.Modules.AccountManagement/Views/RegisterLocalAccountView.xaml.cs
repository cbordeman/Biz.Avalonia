using Biz.Shared.Views;
using Biz.Modules.AccountManagement.ViewModels;

namespace Biz.Modules.AccountManagement.Views;

public partial class RegisterLocalAccountView 
    : FormFactorAwareUserControl<RegisterLocalAccountViewModel>
{
    public RegisterLocalAccountView()
    {
        InitializeComponent();
    }
}
