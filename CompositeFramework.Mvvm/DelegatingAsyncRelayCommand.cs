using System.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CompositeFramework.Mvvm;

/// <summary>
/// An unsealed version of AsyncRelayCommand.  Delegates to an
/// inner AsynbcRelayCommand.
/// </summary>
public class DelegatingAsyncRelayCommand : IAsyncRelayCommand
{
    private readonly AsyncRelayCommand inner;

    public DelegatingAsyncRelayCommand(Func<Task> execute)
    {
        inner = new AsyncRelayCommand(execute);
        HookEvents();
    }

    public DelegatingAsyncRelayCommand(Func<Task> execute, AsyncRelayCommandOptions options)
    {
        inner = new AsyncRelayCommand(execute, options);
        HookEvents();
    }

    public DelegatingAsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute)
    {
        inner = new AsyncRelayCommand(cancelableExecute);
        HookEvents();
    }

    public DelegatingAsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, AsyncRelayCommandOptions options)
    {
        inner = new AsyncRelayCommand(cancelableExecute, options);
        HookEvents();
    }

    public DelegatingAsyncRelayCommand(Func<Task> execute, Func<bool> canExecute)
    {
        inner = new AsyncRelayCommand(execute, canExecute);
        HookEvents();
    }

    public DelegatingAsyncRelayCommand(Func<Task> execute, Func<bool> canExecute, AsyncRelayCommandOptions options)
    {
        inner = new AsyncRelayCommand(execute, canExecute, options);
        HookEvents();
    }

    public DelegatingAsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute)
    {
        inner = new AsyncRelayCommand(cancelableExecute, canExecute);
        HookEvents();
    }

    public DelegatingAsyncRelayCommand(Func<CancellationToken, Task> cancelableExecute, Func<bool> canExecute, AsyncRelayCommandOptions options)
    {
        inner = new AsyncRelayCommand(cancelableExecute, canExecute, options);
        HookEvents();
    }

    private void HookEvents()
    {
        inner.PropertyChanged += (s, e) => PropertyChanged?.Invoke(this, e);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public Task? ExecutionTask => inner.ExecutionTask;
    public bool CanBeCanceled => inner.CanBeCanceled;
    public bool IsCancellationRequested => inner.IsCancellationRequested;
    public bool IsRunning => inner.IsRunning;
    public void Cancel() => inner.Cancel();
    public Task ExecuteAsync(object? parameter) => inner.ExecuteAsync(parameter);
    public void NotifyCanExecuteChanged() => inner.NotifyCanExecuteChanged();
    public bool CanExecute(object? parameter) => inner.CanExecute(parameter);
    public void Execute(object? parameter) => inner.Execute(parameter);
    public event EventHandler? CanExecuteChanged
    {
        add => inner.CanExecuteChanged += value;
        remove => inner.CanExecuteChanged -= value;
    }
}