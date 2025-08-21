using System.Threading.Tasks;
using Biz.Shell.ViewModels;
using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;

namespace Biz.Platform.ViewModels;

public sealed class MessageDialogViewModel : BindableBase, IDialogAware
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

    #region CancelText
    public string? CancelText
    {
        get => cancelText;
        set => SetProperty(ref cancelText, value);
    }
    string? cancelText;        
    #endregion CancelText

    #region OkText
    public string? OkText
    {
        get => okText;
        set => SetProperty(ref okText, value);
    }
    string? okText;        
    #endregion OkText
    
    public void OnDialogOpened(IDialogParameters parameters)
    {
        Title = parameters.GetValue<string>("title");;
        Message = parameters.GetValue<string>("message");
        CancelText = parameters.GetValue<string>("cancelText");
        OkText = parameters.GetValue<string>("okText");
    }

    #region OkCommand
    AsyncDelegateCommand? okCommand;
    public AsyncDelegateCommand OkCommand => okCommand ??= new AsyncDelegateCommand(ExecuteOkCommand, CanOkCommand);
    bool CanOkCommand() => true;
    Task ExecuteOkCommand()
    {
        RequestClose.Invoke(ButtonResult.OK);
        return Task.CompletedTask;
    }
    #endregion OkCommand
    
    #region CancelCommand
    AsyncDelegateCommand? cancelCommand;
    public AsyncDelegateCommand CancelCommand => cancelCommand ??= new AsyncDelegateCommand(ExecuteCancelCommand, CanCancelCommand);
    bool CanCancelCommand() => true;
    Task ExecuteCancelCommand()
    {
        RequestClose.Invoke(ButtonResult.Cancel);
        return Task.CompletedTask;
    }
    #endregion CancelCommand
    
    // Called when the dialog is closed to finalize or clean up.
    public void OnDialogClosed()
    {
        // Clean up resources if needed.
    }

    // Control whether the dialog may be closed. Return false to prevent closing.
    public bool CanCloseDialog() => true;
}
