using Biz.Modules.AccountManagement.ViewModels;
using Biz.Shell.Views;

namespace Biz.Modules.AccountManagement.Views;

public partial class TenantSelectionView 
    : FormFactorAwareUserControl<TenantSelectionViewModel>
{
    public TenantSelectionView() : base()
    {
        InitializeComponent(); 
    }    
}

