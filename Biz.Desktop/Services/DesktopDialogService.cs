using System.Threading.Tasks;
using Biz.Shell.Services;
using CompositFramework.Avalonia.Dialogs;
using ShadUI;

namespace Biz.Desktop.Services;

public class DesktopDialogService(DialogManager dialogManager)
    : IDialogService
{
    public object DialogHost { get; } = dialogManager;

    public Task<bool> Confirm(string title, string message, 
        string okText = "OK", string? cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();
        var dlg = ((DialogManager)DialogHost)
            .CreateDialog(title, message)
            .WithPrimaryButton(okText, () => tcs.SetResult(true))
            .WithCancelButton(cancelText, () => tcs.SetResult(false))
            .WithMinWidth(300)
            .WithMaxWidth(600);
        
        dlg.Show();
        
        return tcs.Task;
    }
}