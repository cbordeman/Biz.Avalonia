using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CompositeFramework.Core;
using CompositeFramework.Core.Location;
using CompositeFramework.Mvvm;
using CompositFramework.Avalonia.Dialogs;

namespace Biz.Mobile.ViewModels;

public sealed class MessageDialogViewModel
    : ViewModelBase, INavigationAware, IDialog<bool?>
{
    // This is set by the dialog service.
    public RequestObject<bool?>? CloseDialogRequest { get; set; }

    public string? Title { get; set => SetProperty(ref field, value); }
    public string? Message { get; set => SetProperty(ref field, value); }
    public string? CancelText { get; set => SetProperty(ref field, value); }
    public string? OkText { get; set => SetProperty(ref field, value); }

    public Task OnNavigatedTo(Dictionary<string, object> parameters)
    {
        Title = parameters["title"] as string;
        Message = parameters["message"] as string;
        CancelText = parameters["cancelText"] as string;
        OkText = parameters["okText"] as string;

        return Task.CompletedTask;
    }

    #region OkCommand
    AsyncRelayCommand? okCommand;
    public AsyncRelayCommand OkCommand => okCommand ??= new AsyncRelayCommand(ExecuteOkCommand, CanOkCommand);
    bool CanOkCommand() => true;
    Task ExecuteOkCommand()
    {
        Debug.Assert(CloseDialogRequest != null);
        return CloseDialogRequest.Invoke(true);
    }
    #endregion OkCommand

    #region CancelCommand
    AsyncRelayCommand? cancelCommand;
    public AsyncRelayCommand CancelCommand => cancelCommand ??= new AsyncRelayCommand(ExecuteCancelCommand, CanCancelCommand);
    bool CanCancelCommand() => true;
    Task ExecuteCancelCommand()
    {
        Debug.Assert(CloseDialogRequest != null);
        return CloseDialogRequest.Invoke(false);
    }
    #endregion CancelCommand

    public void OnDialogClosed(bool? result) { }
}