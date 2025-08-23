using System.Threading.Tasks;
using Biz.Shell.Services;
using ShadUI;

namespace Biz.Desktop.Services;

public class DesktopDialogService(DialogManager dialogManager)
    : IPlatformDialogService
{
    public object DialogHost { get; } = dialogManager;

    public Task<bool> Confirm(string title, string message, 
        string okText = "OK", string? cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();
        var dlg = ((DialogManager)DialogHost)
            .CreateDialog(title, message)
            .WithPrimaryButton(okText, () => tcs.SetResult(true))
            .WithMinWidth(300)
            .WithMaxWidth(600);

        if (cancelText != null) 
            dlg = dlg.WithCancelButton(
                cancelText, () => tcs.SetResult(false),
                buttonStyle: DialogButtonStyle.Secondary);
        dlg.Show();
        
        return tcs.Task;
    }
}