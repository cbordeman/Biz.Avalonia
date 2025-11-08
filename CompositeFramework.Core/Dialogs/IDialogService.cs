namespace CompositeFramework.Core.Dialogs;

public interface IDialogService
{
    Task<bool> Confirm(
        string title, string message,
        string? okText = null, bool showCancel = false,
        string? cancelText = null,
        CompositeDialogOptions? options = null);

    void RegisterDialog<TViewModel, TView>(string? dialogName = null)
        where TViewModel : IDialogViewModel;
    
    /// <summary>
    /// Call by dialog name.  Returns the viewmodel.
    /// </summary>
    Task<IDialogViewModel> Show(
        string? moduleName,
        string dialogName, 
        CompositeDialogOptions? options = null,
        params NavParam[] parameters);
    
    /// <summary>
    /// Pass in the viewmodel.
    /// </summary>
    Task Show(
        string? moduleName,
        IDialogViewModel vm,
        CompositeDialogOptions? options = null,
        params NavParam[] parameters);
    
    /// <summary>
    /// Closes an open dialog.
    /// </summary>
    Task Close(IDialogViewModel dialogViewModel);
}