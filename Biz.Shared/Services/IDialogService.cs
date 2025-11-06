namespace Biz.Shared.Services;

public interface IDialogService
{
    Task<bool> Confirm(
        string title, string message,
        string? okText = null, string? cancelText = null);
    
    void RegisterDialog<TViewModel, TView>(string? dialogName = null)
        where TViewModel: IDialogViewModel
        where TView: Control;

    Task<TReturn> Show<TReturn>(string dialogName, params NavParam[] parameters);
    Task Close(IDialogViewModel dialogViewModel);
}