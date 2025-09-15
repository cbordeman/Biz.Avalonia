using CompositeFramework.Core;

namespace CompositFramework.Avalonia.Dialogs;

public interface IDialog<T>
{
    /// <summary>
    /// Called by the dialog service when the dialog is
    /// closed to finalize or clean up.
    /// </summary>
    /// <returns>Null if the dialog was cancelled.</returns>
    void OnDialogClosed(T? result);
    
    /// <summary>
    /// Invoke to ask dialog service to close the dialog.
    /// </summary>
    RequestObject<T?>? CloseDialogRequest { get; set; }
}
