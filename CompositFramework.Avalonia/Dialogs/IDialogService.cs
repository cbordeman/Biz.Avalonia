namespace CompositFramework.Avalonia.Dialogs;

public interface IDialogService
{
    /// <summary>
    /// Presents a simple OK Cancel dialog with some text.
    /// If null is passed for cancelText, the cancel button is
    /// not displayed.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="okText"></param>
    /// <param name="cancelText">Pass null for no Cancel button.</param>
    /// <returns></returns>
    Task<bool> Confirm(string title, string message,
        string okText = "OK", string? cancelText = "Cancel");
}
