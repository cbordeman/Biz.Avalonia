namespace Biz.Shell.Views;

public partial class TenantSelectionView 
    : TenantSelectionViewBase
{
    public TenantSelectionView() : base()
    {
        InitializeComponent(); 
    }    
}

public class TenantSelectionViewBase : 
    FormFactorAwareUserControl<TenantSelectionViewModel>;