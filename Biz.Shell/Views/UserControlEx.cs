#pragma warning disable AVP1002

using Avalonia.Interactivity;

namespace Biz.Shell.Views;

public class UserControlEx<TViewModel> 
    : UserControl 
    where TViewModel : class
{
    protected UserControlEx() : base()
    {
        DataContextChanged += (_, _) =>
        {
            ViewModel = DataContext as TViewModel;
        };
    }
    
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (DataContext is IOnViewLoaded onViewLoaded)
            onViewLoaded.OnViewLoaded();
    }

    #region ViewModel
    // ReSharper disable once MemberCanBePrivate.Global
    public static readonly DirectProperty<UserControlEx<TViewModel>, TViewModel?> 
        ViewModelProperty =
            AvaloniaProperty.RegisterDirect<UserControlEx<TViewModel>, TViewModel?>(
                nameof(ViewModel), 
                (UserControlEx<TViewModel> o) => (TViewModel)o.DataContext!,
                (UserControlEx<TViewModel> o, TViewModel? v) => 
                    o.ViewModel = (TViewModel)v!);
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
