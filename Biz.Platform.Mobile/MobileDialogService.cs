using System.Threading.Tasks;
using Biz.Core.Services;
using Biz.Platform.Views;
using Prism.Dialogs;

namespace Biz.Platform;

public class MobileDialogService : IPlatformDialogService
{
    // Not used in mobile.
    public object? DialogHost => null;

    private readonly IDialogService dialogService;
    
    public MobileDialogService(IDialogService dialogService)
    {
        this.dialogService = dialogService;
    }

    public Task<bool> Confirm(string title, string message,
        string okText = "OK", string? cancelText = "Cancel")
    {
        var tcs = new TaskCompletionSource<bool>();
        var parameters = new DialogParameters
        {
            { "title", title },
            { "message", message },
            { "okText", okText }
        };
        if (!string.IsNullOrEmpty(cancelText)) 
            parameters.Add("cancelText", cancelText);

        dialogService.ShowDialog(
            nameof(MessageDialogView),
            parameters, 
            (IDialogResult dialogResult) =>
            {
                // Assume your dialog returns ButtonResult.OK or ButtonResult.Cancel
                tcs.SetResult(dialogResult.Result == ButtonResult.OK);
            }
        );
        return tcs.Task;
    }
}