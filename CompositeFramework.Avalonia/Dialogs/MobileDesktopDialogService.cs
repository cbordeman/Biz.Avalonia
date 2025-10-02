using System.ComponentModel;
using Avalonia.Controls;

namespace CompositeFramework.Avalonia.Dialogs;

public class MobileDesktopDialogService : IDialogService
{
    public object? DialogHost { get; set; }
    
    public Task<bool> Confirm(
        string title,
        string message,
        string okText = "OK",
        string? cancelText = "Cancel")
    {
        // Assume mobile mode: inject overlay into main root Panel.
        var tcs = new TaskCompletionSource<bool>();
        
        
        
        return tcs.Task;
    }
    
    public void RegisterDialog<TViewModel, TView>()
        where TViewModel : IDialog, INotifyPropertyChanged where TView : UserControl
    {
        
    }
}