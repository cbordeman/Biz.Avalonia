using ShadUI;

namespace Biz.Core.Services;

public interface IPlatformDialogService
{
    // A ShadUI DialogManager if desktop.
    object? DialogHost { get; }
    
    Task<bool> Confirm(string title, string message,
        string okText = "OK", string? cancelText = "Cancel");
}
