namespace CompositeFramework.ShadUi.Dialogs;

public class ShadUiDialogService : IDialogService
{
    public object? DialogHost { get; set; }
    
    public async Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel")
    {
        if (IsDesktop)
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
    
    public Task<TResult> ShowDialog<TResult>(IDialog dialog)
    {
        // TODO: Implement    
    }
    
    public void RegisterDialogView<TViewModel, TView>()
        where TViewModel : IDialog where TView : class
    {
        // TODO: Implement
    }

    private bool IsDesktop =>
        OperatingSystem.IsWindows() || 
        OperatingSystem.IsMacOS() || 
        OperatingSystem.IsLinux();

    private static void SetDialogHost()
    {
        if (Application.Current == null)
            throw new NullReferenceException("Application.Current was null when getting the main view.");
        
        // Get the root Panel of your application (set as MainView on mobiles per ISingleViewApplicationLifetime)
        switch(Application.Current.ApplicationLifetime)
        {
            case ISingleViewApplicationLifetime single:
                if (single.MainView == null)
                    throw new NullReferenceException(
                        "Application.Current.ApplicationLifetime.MainView " +
                        "was null when getting the main view.");
                return single.MainView;
            default:
                throw new NotSupportedException();
        };
    }
}
