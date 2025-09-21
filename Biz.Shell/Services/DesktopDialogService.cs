using CompositFramework.Avalonia.Dialogs;

namespace Biz.Shell.Services;

public class DesktopDialogService : IDialogService
{
    object? DialogHost { get; }
    public Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel") =>
        Task.FromResult(true);
}
