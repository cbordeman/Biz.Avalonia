using Avalonia.Controls;

namespace CompositeFramework.Avalonia.Dialogs;

public interface IDialogService
{
    Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel");
    
    /// <summary>
    /// Registers a custom dialog view with its DataContext type.
    /// </summary>
    /// <typeparam name="TView">The type of view</typeparam>
    /// <typeparam name="TDialog">The type of the dialog ViewModel</typeparam>
    /// <returns></returns>
    void Register<TView, TDialog>() 
        where TView : Control
        where TDialog : IDialog;
    
    /// <summary>
    /// Closes the dialog associated with the specified context and invokes the
    /// appropriate callbacks.
    /// </summary>
    /// <typeparam name="TDialog">The type of the
    /// dialog ViewModel.</typeparam>
    /// <param name="context">The DataContext of the dialog to close.</param>
    void Close<TDialog>(TDialog context) where TDialog : IDialog;
}