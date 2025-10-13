using System.ComponentModel;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CompositeFramework.Core.Dialogs;

namespace CompositeFramework.Avalonia.Dialogs;

public class AvaloniaDialogService : IDialogService
{
    static Control DialogHost;
    
    static AvaloniaDialogService()
    {
        SetTopLevel();
    }
    
    public async Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel")
    {
        if (IsDesktop)
        {
            // Use ShadUI/DialogHost modal dialog on desktop
            return await ShadUiDialogManager.Current.ShowConfirmAsync(title, message, okText, cancelText);
        }
        else
        {
            var tcs = new TaskCompletionSource<bool>();
            var mainView = GetMainView();
            
            try
            {
                var confirmView = new ConfirmDialogView()
                {
                    DataContext = new ConfirmDialogViewModel(title, message, okText, cancelText, tcs)
                };

                // Add the overlay visually
                mainView.Children.Add(confirmView); 

                var result = await tcs.Task;                
            }
            finally
            {
                mainView.Children.Remove(confirmView);
                return result;
            }
        }
    }
    
    public void ChangeDialogContainer<TDialogContainer>(int a)
        where TDialogContainer : IDialogContainer
    {
        
    }
    
    public void OverrideDialogContainer<TDialogContainer>(int a)
        where TDialogContainer : IDialogContainer
    {
        
    }

    public void RegisterDialog<TView, TViewModel>()
        where TView
        where TViewModel : IDialogViewModel
    {
        throw new NotImplementedException();
    }
    
    void IDialogService.Show<TResult>(IDialogViewModel dialog)
    {
        throw new NotImplementedException();
    }
    public async Task<TResult> ShowModal<TResult>(IDialogViewModel dialog)
    {
        throw new NotImplementedException();
    }

    public Task<TResult> Show<TResult>(IDialogViewModel dialog)
    {
        // TODO: Implement    
    }
    
    private bool IsDesktop =>
        OperatingSystem.IsWindows() || 
        OperatingSystem.IsMacOS() || 
        OperatingSystem.IsLinux();

    private static void SetDialogHost()
    {
        if (Application.Current == null)
            throw new NullReferenceException("Application.Current was null when getting the main view.");
        
        // Get the root Panel of your application (set as MainView on mobiles per ISingleViewApplicationLifetime)
        switch(Application.Current.ApplicationLifetime)
        {
            case ISingleViewApplicationLifetime single:
                if (single.MainView == null)
                    throw new NullReferenceException(
                        "Application.Current.ApplicationLifetime.MainView " +
                        "was null when getting the main view.");
                return single.MainView;
            default:
                throw new NotSupportedException();
        };
    }
}