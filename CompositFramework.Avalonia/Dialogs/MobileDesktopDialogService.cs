namespace CompositFramework.Avalonia.Dialogs;

public class MobileDesktopDialogService : IDialogService
{
    public Task<bool> Confirm(
        string title,
        string message,
        string okText = "OK",
        string? cancelText = "Cancel")
    {
        // Assume mobile mode: inject overlay into main root Panel.
        var tcs = new TaskCompletionSource<bool>();
        
        
        
        return tcs.Task;
    }
}