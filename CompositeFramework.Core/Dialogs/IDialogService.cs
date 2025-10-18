namespace CompositeFramework.Core.Dialogs;

public interface IDialogService
{
    Task<bool> Confirm(string title, string message, string okText = "OK", string? cancelText = "Cancel");

    // /// <summary>
    // /// Registers an alternative class to provide the 
    // /// dialog container.  If not called, the DialogWindow
    // /// or DialogUserControl class will be used depending
    // /// on the platform.</summary>
    // void RegisterDialogContainer<TDialogContainer>() 
    //     where TDialogContainer : IDialogContainer;
    //
    // /// <summary>
    // /// Registers a custom dialog View with its ViewModel type.
    // /// </summary>
    // void RegisterDialog<TView, TViewModel>() 
    //     where TView
    //     where TViewModel : IDialogViewModel;
    //
    // void Show<TResult>(IDialogViewModel dialog);
    // Task<TResult> ShowModal<TResult>(IDialogViewModel dialog);
}