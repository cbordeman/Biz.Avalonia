using System;
using System.Threading.Tasks;
using Android.App;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core.Navigation;

namespace Biz.Shell.Android.Services;

public class AndroidDialogService : BaseDialogService
{
    public override Task<bool> Confirm(
        string title, string message,
        string? okText = null,
        bool showCancel = false,
        string? cancelText = null,
        // options ignored in this implementation
        CompositeDialogOptions? options = null)
    {
        TaskCompletionSource<bool> tcs = new();

        var activity = MainActivity.GetActivity!();

        var dialog = new AlertDialog.Builder(activity);
        dialog.SetTitle(title);
        dialog.SetMessage(message);
        dialog.SetPositiveButton(okText ?? "OK",
            (_, _) => tcs.SetResult(true));
        if (showCancel)
            dialog.SetNegativeButton(cancelText ?? "Cancel",
                (_, _) => tcs.SetResult(false));
        dialog.Show();

        return tcs.Task;
    }

    public override void RegisterDialog<TViewModel, TView>(string? dialogName = null)
    {
        throw new NotImplementedException();
    }

    public override async Task<IDialogViewModel> Show(string? moduleName, string dialogName, CompositeDialogOptions? options = null, params NavParam[] parameters)
    {
        throw new NotImplementedException();
    }
    
    public override async Task Show(string? moduleName, IDialogViewModel vm, CompositeDialogOptions? options = null, params NavParam[] parameters)
    {
        throw new NotImplementedException();
    }
    
    public override async Task Close(IDialogViewModel dialogViewModel)
    {
        throw new NotImplementedException();
    }
}
