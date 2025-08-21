using ShadUI;

namespace Biz.Shell.Services;

public interface IPlatformDialogService
{
    // A ShadUI DialogManager if desktop.
    object? DialogHost { get; }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="title"></param>
    /// <param name="message"></param>
    /// <param name="okText">Defaults to OK.</param>
    /// <param name="cancelText">Set to null for no cancel button.</param>
    /// <returns></returns>
    Task<bool> Confirm(string title, string message,
        string okText = "OK", string? cancelText = "Cancel");
}
