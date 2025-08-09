using Biz.Core.ViewModels;
using Prism.Mvvm;

namespace Biz.Platform.ViewModels;

public class MessageDialogViewModel : BindableBase, IDialogAware
{
    // The Dialog Service sets this property for you, do NOT initialize it yourself.
    public DialogCloseListener RequestClose { get; private set; }

    #region Title
    public string? Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }
    string? title;        
    #endregion Title

    #region Message
    public string? Message
    {
        get => message;
        set => SetProperty(ref message, value);
    }
    string? message;        
    #endregion Message
    
    // Called when the dialog opens; use this to read parameters and initialize state.
    public void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title");;
        Message = parameters.GetValue<string>("message");
    }

    // Called when the dialog is closed to finalize or clean up.
    public void OnDialogClosed()
    {
        // Clean up resources if needed.
    }

    // Control whether the dialog may be closed. Return false to prevent closing.
    public bool CanCloseDialog()
    {
        // Example: only allow closing if a message exists.
        return !string.IsNullOrEmpty(Message);
    }
}
