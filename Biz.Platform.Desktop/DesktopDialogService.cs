using Biz.Core.Services;
using ShadUI;

namespace Biz.Platform;

public class DesktopDialogService : IPlatformDialogService
{
    public object DialogHost { get; }

    public DesktopDialogService(DialogManager dialogManager)
    {
        this.DialogHost = dialogManager;
    }

    public Task<bool> Confirm(string title, string message, 
        string okText = "OK", string? cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();
        var dlg = ((DialogManager)DialogHost)
            .CreateDialog(title, message)
            .WithPrimaryButton(okText, () => tcs.SetResult(true))
            .WithMinWidth(300);

        if (cancelText != null) 
            dlg = dlg.WithCancelButton(cancelText, () => tcs.SetResult(false));
        dlg.Show();
        
        return tcs.Task;
    }

    public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
    {
        throw new NotImplementedException();
    }
}