using Biz.Shell.Views;
using Biz.Modules.AccountManagement.ViewModels;

namespace Biz.Modules.AccountManagement.Views;

public partial class ResetPasswordAfterEmailConfirmationView 
    : FormFactorAwareUserControl<ResetPasswordAfterEmailConfirmationViewModel>
{
    public ResetPasswordAfterEmailConfirmationView()
    {
        InitializeComponent();
    }
}
