using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CompositeFramework.Avalonia.Dialogs;
using CompositeFramework.Core;
using CompositeFramework.Core.Dialogs;
using CompositeFramework.Core.Navigation;
using CompositeFramework.Core.ViewModels;

namespace Biz.Mobile.ViewModels;

public sealed class MessageDialogViewModel
    : BindingValidatingBase, IDialog
{
    public IContextNavigationService? NavigationService { get; set; }
    
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
        return CloseDialogRequest.PublishAsync(true);
    }
    #endregion OkCommand

    #region CancelCommand
    AsyncRelayCommand? cancelCommand;
    public AsyncRelayCommand CancelCommand => cancelCommand ??= new AsyncRelayCommand(ExecuteCancelCommand, CanCancelCommand);
    bool CanCancelCommand() => true;
    Task ExecuteCancelCommand()
    {
        return CloseDialogRequest.PublishAsync(false);
    }
    #endregion CancelCommand

    public void OnDialogClosed(bool? result) { }

    public AsyncEvent<bool?> CloseDialogRequest { get; set; } = new();

    public Task OnNavigatedToAsync(NavigationContext context)
    {
        return Task.CompletedTask;
    }
}