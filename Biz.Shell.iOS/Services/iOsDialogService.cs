using System;
using System.Threading.Tasks;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core.Navigation;
using UIKit;

namespace Biz.Shell.iOS.Services;

// ReSharper disable once InconsistentNaming
public class iOsDialogService : BaseDialogService
{
    public override Task<bool> Confirm(
        string title, string message, 
        string? okText = null, 
        bool showCancel = false, 
        string? cancelText = null,
        // ignored in this implementation
        CompositeDialogOptions? options = null)
    {
        UIViewController controller = null!;

        var tcs = new TaskCompletionSource<bool>();

        var alert = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);
        alert.AddAction(UIAlertAction.Create(okText ?? "OK",
            UIAlertActionStyle.Default,
            _ => tcs.SetResult(true)));
        if (showCancel)
            alert.AddAction(UIAlertAction.Create(cancelText ?? "Cancel",
                UIAlertActionStyle.Cancel,
                _ => tcs.SetResult(false)));
        controller.PresentViewController(alert, true, null);

        return tcs.Task;
    }
    public override void RegisterDialog<TViewModel, TView>(string? dialogName = null)
    {
        throw new NotImplementedException();
    }

    public override Task<IDialogViewModel> Show(
        string? moduleName,
        string dialogName,
        CompositeDialogOptions? options = null,
        params NavParam[] parameters)
    {
        throw new NotImplementedException();
    }

    public override Task Show(
        string? moduleName,
        IDialogViewModel vm,
        CompositeDialogOptions? options = null,
        params NavParam[] parameters)
    {
        throw new NotImplementedException();
    }

    public override Task Close(
        IDialogViewModel dialogViewModel)
    {
        throw new NotImplementedException();
    }
}
