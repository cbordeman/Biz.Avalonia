using ShadUI;

namespace Biz.Shared.Services;

public class ShadUiDialogService(DialogManager dialogManager) 
    : IDialogService
{
    /// <summary>
    /// This should be set, or bound in XAML.
    /// </summary>
    public object? DialogHost { get; set; }

    readonly Dictionary<string, ViewModelViewBinding> dialogNameMap = new();

    public Task<bool> Confirm(
        string title, string message, 
        string? okText = null, string? cancelText = null)
    {
        var tcs = new TaskCompletionSource<bool>();
        
        var dialogBuilder = dialogManager
            .CreateDialog(title, message)
            .WithPrimaryButton(
                okText ?? "OK", 
                () => tcs.TrySetResult(true))
            .WithMinWidth(300)
            .Dismissible();

        if (cancelText != null)
            dialogBuilder
                .WithCancelButton(cancelText ?? "Cancel",
                    () => tcs.TrySetResult(false));

        dialogBuilder
            .Show();
        
        return tcs.Task;
    }
    
    public void RegisterDialog<TViewModel, TView>(
        string? dialogName = null)
        where TViewModel : IDialogViewModel
        where TView : Control
    {
        dialogName ??= typeof(TViewModel).AssemblyQualifiedName;
        if (dialogName == null)
            throw new InvalidOperationException("Dialog Name is null and type has no AssemblyQualifiedName.");
        if (dialogNameMap.ContainsKey(dialogName!))
            throw new InvalidOperationException($"Dialog name \"{dialogName}\" already registered.");
        dialogNameMap.Add(dialogName,
            new ViewModelViewBinding(typeof(TViewModel), typeof(TView)));

        DialogManager dm1 = new();
#pragma warning disable SPLATDI001
        dm1.Register<TView, TViewModel>();
#pragma warning restore SPLATDI001
    }

    public async Task<IDialogViewModel> Show(string dialogName, params NavParam[] parameters)
    {
        if (!dialogNameMap.TryGetValue(dialogName, out var value))
            throw new InvalidOperationException($"Dialog Name \"{dialogName}\" not registered.");
        var (vmType, viewType) = value;
        var vm = Locator.Current.Resolve(vmType);
        var view = Locator.Current.Resolve(viewType);
        if (vm is not IDialogViewModel dlgVm)
            throw new InvalidOperationException($"Dialog ViewModel \"{vmType.AssemblyQualifiedName}\" does not implement {nameof(IDialogViewModel)}.");
        
        dialogManager.CreateDialog(vm)
            .Dismissible()
            .Show();
        
        await dlgVm.OpenedAsync(parameters);
        
        return dlgVm;
    }

    public async Task Show(IDialogViewModel vm, params NavParam[] parameters)
    {
        ArgumentNullException.ThrowIfNull(vm);
        
        var viewType = dialogNameMap.Values.FirstOrDefault(v => v.ViewModelType == vm.GetType())?.ViewType; 
        if (viewType == null)
            throw new InvalidOperationException($"Dialog ViewModel " +
                $"{vm.GetType().AssemblyQualifiedName}\" not registered " +
                $"in Dialog Service.");
        
        dialogManager.CreateDialog(vm)
            .Dismissible()
            .Show();
        
        await vm.OpenedAsync(parameters);
    }

    public async Task Close(IDialogViewModel dialogViewModel)
    {
        dialogManager.Close(dialogViewModel);
        await dialogViewModel.ClosedAsync();
    }
}