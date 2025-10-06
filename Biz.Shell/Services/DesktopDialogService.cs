using CompositeFramework.Avalonia.Dialogs;

namespace Biz.Shell.Services;

public class DesktopDialogService : IDialogService
{
    public object? DialogHost { get; set; }
    public Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel") =>
        Task.FromResult(true);
}
