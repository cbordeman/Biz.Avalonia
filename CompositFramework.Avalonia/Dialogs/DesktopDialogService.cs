namespace CompositFramework.Avalonia.Dialogs;

public class DesktopDialogService : IDialogService
{
    object? DialogHost { get; }
    public Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel")
    {
        return Task.FromResult(true);
    }
}
