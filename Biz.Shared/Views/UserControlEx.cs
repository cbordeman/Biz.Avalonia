#pragma warning disable AVP1002

using Avalonia.Interactivity;

namespace Biz.Shared.Views;

public class UserControlEx<TViewModel> : UserControl 
    where TViewModel : class
{
    protected UserControlEx() : base()
    {
        DataContextChanged += (_, _) => 
            ViewModel = DataContext as TViewModel;
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is IOnViewLoaded onViewLoaded)
            onViewLoaded.OnViewLoaded();
    }

    #region ViewModel
    
    // ViewModel is not responsible for setting the VM on 
    // user controls.  It is here as a binding target for
    // scenarios when binding to DataContext explicitly is
    // necessary, but it isn't strongly typed so compile
    // time binding fails. 
    
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly DirectProperty
        <UserControlEx<TViewModel>, TViewModel?> 
        ViewModelProperty =
            AvaloniaProperty.RegisterDirect
                <UserControlEx<TViewModel>, TViewModel?>(
                nameof(ViewModel),
                // Use DataContext as the backing instead of
                // the INPC property or things don't get set
                // quickly enough and the ViewModel is null
                // when the binding system reads it.
                o => (TViewModel)o.DataContext!,
                (o, v) => o.ViewModel = (TViewModel)v!);
    public TViewModel? ViewModel
    {
        get;
        set
        {
            if (SetAndRaise(ViewModelProperty, ref field, value))
                DataContext = value;
        }
    }
    #endregion ViewModel
}
