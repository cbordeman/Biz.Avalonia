using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CompositeFramework.Core.Dialogs;
using MsBox.Avalonia;
using MsBox.Avalonia.Dto;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia.Models;

namespace CompositeFramework.Avalonia.Dialogs;

public class AvaloniaDialogService : IDialogService
{
    //static Control DialogHost;

    // static AvaloniaDialogService()
    // {
    //     //SetTopLevel();
    // }

    public async Task<bool> Confirm(
        string title,
        string message,
        string okText = "OK",
        string? cancelText = "Cancel")
    {
        var lifetime = Application.Current?.ApplicationLifetime;

        var boxParams = new MessageBoxCustomParams
        {
            ContentTitle = title,
            ContentMessage = message,
            Icon = Icon.Question,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            ShowInCenter = true,
            SizeToContent = SizeToContent.WidthAndHeight,
            ButtonDefinitions =
            [
                new ButtonDefinition
                {
                    Name = okText,
                    IsDefault = true
                },
                new ButtonDefinition
                {
                    Name = cancelText ?? "Cancel",
                    IsCancel = true
                }
            ]
        };

        var box = MessageBoxManager.GetMessageBoxCustom(boxParams);

        string result = lifetime switch
        {
            IClassicDesktopStyleApplicationLifetime desktop => 
                await box.ShowWindowDialogAsync(desktop.MainWindow!),
            ISingleViewApplicationLifetime => 
                await box.ShowAsync(),
            _ => await box.ShowAsync()
        };

        return result == okText;
    }
}

// public async Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel")
// {
//     public static async Task<bool> Confirm(
//         string title,
//         string message,
//         string okText = "OK",
//         string? cancelText = "Cancel")
//     {
//         var lifetime = Application.Current?.ApplicationLifetime;
//         var box = MessageBoxManager.GetMessageBoxCustom(
//             new MessageBox.Avalonia.DTO.MessageBoxCustomParams
//             {
//                 ContentTitle = title,
//                 ContentMessage = message,
//                 ButtonDefinitions = new()
//                 {
//                     new MessageBox.Avalonia.DTO.Models.ButtonDefinition { Name = okText },
//                     new MessageBox.Avalonia.DTO.Models.ButtonDefinition { Name = cancelText ?? "Cancel" }
//                 },
//                 Icon = MessageBox.Avalonia.Enums.Icon.Question,
//                 WindowStartupLocation = WindowStartupLocation.CenterOwner,
//                 Topmost = true
//             });
//
//         // Show based on platform window type
//         string? result;
//         if (lifetime is IClassicDesktopStyleApplicationLifetime desktop)
//         {
//             result = await box.ShowWindowDialogAsync(desktop.MainWindow!);
//         }
//         else if (lifetime is ISingleViewApplicationLifetime)
//         {
//             result = await box.ShowAsPopupAsync();
//         }
//         else
//         {
//             result = await box.ShowAsync();
//         }
//
//         return result == okText;
//     }

// if (IsDesktop)
// {
//     // Use ShadUI/DialogHost modal dialog on desktop
//     DialogManager.CreateDialog("Close", "Do you really want to exit?")
//         .WithPrimaryButton("Yes", OnAcceptExit)
//         .WithCancelButton("No")
//         .WithMinWidth(300)
//         .Show();
// }
// else
// {
//     var tcs = new TaskCompletionSource<bool>();
//     var mainView = GetMainView();
//     
//     try
//     {
//         var confirmView = new ConfirmDialogView()
//         {
//             DataContext = new ConfirmDialogViewModel(title, message, okText, cancelText, tcs)
//         };
//
//         // Add the overlay visually
//         mainView.Children.Add(confirmView); 
//
//         var result = await tcs.Task;                
//     }
//     finally
//     {
//         mainView.Children.Remove(confirmView);
//         return result;
//     }
// }


// public void ChangeDialogContainer<TDialogContainer>(int a)
//     where TDialogContainer : IDialogContainer
// {
//     
// }
//
// public void OverrideDialogContainer<TDialogContainer>(int a)
//     where TDialogContainer : IDialogContainer
// {
//     
// }
//
// public void RegisterDialog<TView, TViewModel>()
//     where TView
//     where TViewModel : IDialogViewModel
// {
//     throw new NotImplementedException();
// }
//
// void IDialogService.Show<TResult>(IDialogViewModel dialog)
// {
//     throw new NotImplementedException();
// }
// public async Task<TResult> ShowModal<TResult>(IDialogViewModel dialog)
// {
//     throw new NotImplementedException();
// }
//
// public Task<TResult> Show<TResult>(IDialogViewModel dialog)
// {
//     // TODO: Implement    
// }
//
// private bool IsDesktop =>
//     OperatingSystem.IsWindows() || 
//     OperatingSystem.IsMacOS() || 
//     OperatingSystem.IsLinux();
//
// private static void SetDialogHost()
// {
//     if (Application.Current == null)
//         throw new NullReferenceException("Application.Current was null when getting the main view.");
//     
//     // Get the root Panel of your application (set as MainView on mobiles per ISingleViewApplicationLifetime)
//     switch(Application.Current.ApplicationLifetime)
//     {
//         case ISingleViewApplicationLifetime single:
//             if (single.MainView == null)
//                 throw new NullReferenceException(
//                     "Application.Current.ApplicationLifetime.MainView " +
//                     "was null when getting the main view.");
//             return single.MainView;
//         default:
//             throw new NotSupportedException();
//     };
// }
//}
