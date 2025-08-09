using Biz.Core.Services;
using Biz.Platform.Views;

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
            { "message", message }
        };

        dialogService.ShowDialog(
            nameof(MessageDialogView)
            parameters,
            dialogResult =>
            {
                // Assume your dialog returns ButtonResult.OK or ButtonResult.Cancel
                tcs.SetResult(dialogResult.Result == ButtonResult.OK);
            }
        );
        return tcs.Task;
    }
}