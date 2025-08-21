using System.Threading.Tasks;
using Biz.Shell.Services;
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
            .WithMinWidth(300)
            .WithMaxWidth(500);

        if (cancelText != null) 
            dlg = dlg.WithCancelButton(cancelText, () => tcs.SetResult(false));
        dlg.Show();
        
        return tcs.Task;
    }
}