using ShadUI;

namespace Biz.Shell.Services;

public interface IPlatformDialogService
{
    // A ShadUI DialogManager if desktop.
    // Null for others.
    object? DialogHost { get; }
    
    /// <summary>
    /// Presents a simple OK Cancel dialog with some text.
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="okText"></param>
    /// <param name="cancelText">Pass null for no Cancel button.</param>
    /// <returns></returns>
    Task<bool> Confirm(string title, string message,
        string okText = "OK", string? cancelText = "Cancel");
}
