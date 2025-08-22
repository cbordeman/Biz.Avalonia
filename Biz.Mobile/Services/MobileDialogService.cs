using System.Threading.Tasks;
using Biz.Shell.Services;
using Prism.Dialogs;
using MessageDialogView = Biz.Mobile.Views.MessageDialogView;

namespace Biz.Mobile.Services;

public class MobileDialogService : IPlatformDialogService
{
    // Not used in mobile because we're using Prism
    // dialogs instead of ShadUI dialogs.
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
            dialogResult =>
            {
                // Assume your dialog returns ButtonResult.OK or ButtonResult.Cancel
                tcs.SetResult(dialogResult.Result == ButtonResult.OK);
            }
        );
        return tcs.Task;
    }
}