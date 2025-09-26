namespace Biz.Modules.AccountManagement.ViewModels
{
    [UsedImplicitly]
    public class ResetPasswordAfterEmailConfirmationViewModel : PageViewModelBase
    {
        public ResetPasswordAfterEmailConfirmationViewModel()
        {
            Title = "Reset Password";
        }
        
        public override string Area => "Account";
    }
}
