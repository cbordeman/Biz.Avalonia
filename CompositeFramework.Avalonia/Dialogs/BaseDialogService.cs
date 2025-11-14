namespace CompositeFramework.Avalonia.Dialogs;

public abstract class BaseDialogService : IDialogService
{
    readonly Dictionary<string, Type> dialogHostRegistry = new();

    public void RegisterDialogHost<T>(string name)
        where T : ContentControl
    {
        if (dialogHostRegistry.TryAdd(name, typeof(T)))
            throw new InvalidOperationException($"Dialog control with name {name} already registered");
    }
    
    public abstract Task<bool> Confirm(
        string title, 
        string message, 
        string? okText = null, 
        bool showCancel = false, 
        string? cancelText = null, 
        CompositeDialogOptions? options = null);

    public abstract void RegisterDialog<TViewModel, TView>(
        string? dialogName = null)
        where TViewModel : IDialogViewModel where TView : Control;

    public abstract Task<IDialogViewModel> Show(
        string? moduleName, 
        string dialogName, 
        CompositeDialogOptions? options = null,
        params NavParam[] parameters);
    
    public abstract Task Show(string? moduleName, IDialogViewModel vm, CompositeDialogOptions? options = null, params NavParam[] parameters);
    public abstract Task Close(IDialogViewModel dialogViewModel);
}