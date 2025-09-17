namespace CompositFramework.Avalonia.Dialogs;

public interface IDialog<T> : ILocation
{
    /// <summary>
    /// Set by the dialog service.  Do not set it yourself.
    /// </summary>
    IContextNavigationService? NavigationService { get; set; }
    
    /// <summary>
    /// Called by the dialog service when the dialog is
    /// closed.  Use to finalize or clean up.
    /// </summary>
    void OnDialogClosed(T? result);
    
    /// <summary>
    /// Invoke to ask dialog service to close the dialog.
    /// Set by the dialog service.  Do not set it yourself.
    /// </summary>
    AsyncEvent<T?>? CloseDialogRequest { get; set; }
}