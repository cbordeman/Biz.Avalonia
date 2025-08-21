using Avalonia.Interactivity;

namespace Biz.Shell.Views;

public class UserControlEx : UserControl
{
    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        
        if (this.DataContext is IOnViewLoaded onViewLoaded)
            onViewLoaded.OnViewLoaded();
    }
}