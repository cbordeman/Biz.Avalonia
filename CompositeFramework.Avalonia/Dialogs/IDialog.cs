namespace CompositeFramework.Avalonia.Dialogs;

public interface IDialog
{
    /// <summary>
    /// Set by the dialog service.  Do not set it yourself.
    /// </summary>
    IContextNavigationService? DialogNavigationService { get; set; }
    
    /// <summary>
    /// Called by the dialog service when the dialog is
    /// closed.  Use to finalize or clean up.
    /// </summary>
    void OnDialogClosed(bool? result);

    /// <summary>
    /// Invoke to ask dialog service to close the dialog.
    /// Set by the dialog service.  Do not set it yourself.
    /// </summary>
    AsyncEvent<bool?> CloseDialogRequest { get; }
}