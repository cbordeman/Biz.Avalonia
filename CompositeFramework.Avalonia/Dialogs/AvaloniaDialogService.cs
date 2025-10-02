using Avalonia.Controls;

namespace CompositeFramework.Avalonia.Dialogs;

public class AvaloniaDialogService : IDialogService
{
    public object? DialogHost => IsDesktop() ? ShadUiDialogManager.Current : null;

    public void RegisterDialog<TViewModel, TView>()
        where TViewModel : IDialog, INotifyPropertyChanged where TView : UserControl
    {
        // TODO: implement
    }

    public async Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel")
    {
        if (IsDesktop())
        {
            // Use ShadUI/DialogHost modal dialog on desktop
            return await ShadUiDialogManager.Current.ShowConfirmAsync(title, message, okText, cancelText);
        }
        else
        {
            var tcs = new TaskCompletionSource<bool>();
            var mainView = GetMainView();
            
            try
            {
                var confirmView = new ConfirmDialogView()
                {
                    DataContext = new ConfirmDialogViewModel(title, message, okText, cancelText, tcs)
                };

                // Add the overlay visually
                mainView.Children.Add(confirmView); 

                var result = await tcs.Task;                
            }
            finally
            {
                mainView.Children.Remove(confirmView);
                return result;
            }
        }
    }
    
    private bool IsDesktop() =>
        OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() || OperatingSystem.IsLinux();

    private static Panel GetMainView()
    {
        // Get the root Panel of your application (set as MainView on mobiles per ISingleViewApplicationLifetime)
        return (Panel)Avalonia.Application.Current!.ApplicationLifetime switch
        {
            ISingleViewApplicationLifetime single => single.MainView,
            _ => throw new NotSupportedException()
        };
    }
}
