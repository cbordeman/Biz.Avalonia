using CommunityToolkit.Mvvm.ComponentModel;
using CompositeFramework.Avalonia.Dialogs;
using ShadUI;

namespace Biz.Shared.Services;

public class ShadUiDialogService(ModuleManager moduleManager,
    DialogManager dialogManager) 
    : ObservableObject, IDialogService
{

    /// <summary>
    /// This should be set, or bound in XAML.
    /// </summary>
    public DialogManager DialogManager { get; set; } = dialogManager;

    readonly Dictionary<string, Type> dialogNameViewModelMap = new();

    public Task<bool> Confirm(
        string title, string message, 
        string? okText = null, bool showCancel = false,
        string? cancelText = null,
        CompositeDialogOptions? options = null)
    {
        var tcs = new TaskCompletionSource<bool>();

        var dialogBuilder = DialogManager
            .CreateDialog(title, message)
            .WithPrimaryButton(
                okText ?? "OK",
                () => tcs.TrySetResult(true)) ;

        if (showCancel)
        {
            dialogBuilder =
                dialogBuilder
                    .WithCancelButton(cancelText ?? "Cancel",
                        () => tcs.TrySetResult(false));
        }
        else
        {
            dialogBuilder = dialogBuilder
                .WithCancelButton(null);
        }
        
        dialogBuilder.Show();
        
        return tcs.Task;
    }

    static DialogBuilder<T> SetDialogOptions<T>(
        DialogBuilder<T> db,
        CompositeDialogOptions? options = null)
    {
        if (options == null)
            return db;
        
        if (options.MinWidth > 0)
            db = db.WithMinWidth(options.MinWidth);
        if (options.MaxWidth > 0)
            db = db.WithMaxWidth(options.MaxWidth);

        return db;
    }

    public void RegisterDialog<TViewModel, TView>(
        string? dialogName = null)
        where TViewModel : IDialogViewModel where TView : Control
    {
        dialogName ??= typeof(TViewModel).AssemblyQualifiedName;
        if (dialogName == null)
            throw new InvalidOperationException("Dialog Name is null and type has no AssemblyQualifiedName.");
        if (dialogNameViewModelMap.ContainsKey(dialogName))
            throw new InvalidOperationException($"Dialog name \"{dialogName}\" already registered.");
        dialogNameViewModelMap.Add(dialogName, typeof(TViewModel));

        DialogManager dm1 = new();
#pragma warning disable SPLATDI001
        dm1.Register<TView, TViewModel>();
#pragma warning restore SPLATDI001
    }

    public async Task<IDialogViewModel> Show(
        string? moduleName,
        string dialogName, 
        CompositeDialogOptions? options = null,
        params NavParam[] parameters)
    {
        if (moduleName != null)
            await moduleManager.LoadModuleAsync(moduleName);
        
        if (!dialogNameViewModelMap.TryGetValue(dialogName, out var value))
            throw new InvalidOperationException($"Dialog Name \"{dialogName}\" not registered.");
        var vmType = value;
        var vm = Locator.Current.Resolve(vmType);
        if (vm is not IDialogViewModel dlgVm)
            throw new InvalidOperationException($"Dialog ViewModel \"{vmType.AssemblyQualifiedName}\" does not implement {nameof(IDialogViewModel)}.");
        
        var db = DialogManager.CreateDialog(dlgVm);
        db = SetDialogOptions(db);
        db.Show();
        
        await dlgVm.OpenedAsync(parameters);
        
        return dlgVm;
    }

    public async Task Show(
        string? moduleName,
        IDialogViewModel vm, 
        CompositeDialogOptions? options = null,
        params NavParam[] parameters)
    {
        ArgumentNullException.ThrowIfNull(vm);

        if (moduleName != null)
            await moduleManager.LoadModuleAsync(moduleName);
        
        var db = DialogManager.CreateDialog(vm);
        SetDialogOptions(db);
        db.Show();

        await vm.OpenedAsync(parameters);
    }

    public async Task Close(IDialogViewModel dialogViewModel)
    {
        DialogManager.Close(dialogViewModel);
        await dialogViewModel.ClosedAsync();
    }
}