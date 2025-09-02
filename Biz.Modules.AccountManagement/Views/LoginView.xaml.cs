using Biz.Modules.AccountManagement.ViewModels;
using Biz.Shell.Views;

namespace Biz.Modules.AccountManagement.Views;

public partial class LoginView 
    : FormFactorAwareUserControl<LoginViewModel>
{
    public LoginView() : base()
    {
        InitializeComponent();
    }
}