using JetBrains.Annotations;
using IContainer = DryIoc.IContainer;

namespace Biz.Modules.AccountManagement.ViewModels
{
    [UsedImplicitly]
    public class ResetPasswordAfterEmailConfirmationViewModel : PageViewModelBase
    {
        public ResetPasswordAfterEmailConfirmationViewModel(IContainer container) 
            : base(container)
        {
        }
    }
}
