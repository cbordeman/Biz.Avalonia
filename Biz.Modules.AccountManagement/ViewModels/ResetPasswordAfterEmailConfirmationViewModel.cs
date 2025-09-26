namespace Biz.Modules.AccountManagement.ViewModels
{
    [UsedImplicitly]
    public class ResetPasswordAfterEmailConfirmationViewModel : PageViewModelBase
    {
        public ResetPasswordAfterEmailConfirmationViewModel(IContainer container) 
            : base(container)
        {
        }
        public override string Area => "Account";
    }
}
