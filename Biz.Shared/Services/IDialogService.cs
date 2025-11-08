namespace Biz.Shared.Services;

public interface IDialogService
{
    Task<bool> Confirm(
        string title, string message,
        string? okText = null, string? cancelText = null);
    
    void RegisterDialog<TViewModel, TView>(string? dialogName = null)
        where TViewModel: IDialogViewModel
        where TView: Control;

    /// <summary>
    /// Call by dialog name.  Returns the viewmodel.
    /// </summary>
    Task<IDialogViewModel> Show(
        string moduleName,
        string dialogName, params NavParam[] parameters);
    
    /// <summary>
    /// Pass in the viewmodel.
    /// </summary>
    Task Show(
        string moduleName,
        IDialogViewModel vm, params NavParam[] parameters);
    
    /// <summary>
    /// Closes an open dialog.
    /// </summary>
    Task Close(IDialogViewModel dialogViewModel);
}