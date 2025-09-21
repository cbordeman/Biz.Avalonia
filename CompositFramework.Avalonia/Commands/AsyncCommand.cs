using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Windows.Input;
using Avalonia.Threading;

// ReSharper disable FieldCanBeMadeReadOnly.Local

namespace CompositFramework.Avalonia.Commands;

public class AsyncCommand : ICommand, INotifyPropertyChanged
{
    private bool isExecuting;

    /// <summary>
    /// When inheriting, set to your own method.
    /// </summary>
    protected Func<Task>? ExecuteHandler;
    
    /// <summary>
    /// When inheriting, set or leave null for true.
    /// </summary>
    protected readonly Func<bool>? CanExecuteHandler;
    
    /// <summary>
    /// Use this constructor when inheriting, then set the
    /// properties manually.  This bypasses the C# issue of passing
    /// a non-static method to the base constructor.
    /// </summary>
    public AsyncCommand() { }
    
    public AsyncCommand(Func<Task> execute, Func<bool>? canExecute = null)
    {
        this.ExecuteHandler = execute ?? throw new ArgumentNullException(nameof(execute));
        this.CanExecuteHandler = canExecute;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler? CanExecuteChanged;

    public bool IsExecuting
    {
        get => isExecuting;
        private set
        {
            if (isExecuting == value) return;
            isExecuting = value;
            OnPropertyChanged();
        }
    }

    public bool CanExecute(object? parameter)
    {
        return !IsExecuting && (CanExecuteHandler?.Invoke() ?? true);
    }

    public async void Execute(object? parameter)
    {
        try
        {
            IsExecuting = true;
            RaiseCanExecuteChanged();
            
            if (ExecuteHandler == null)
                throw new InvalidOperationException(
                    $"{nameof(ExecuteHandler)} must be set.");
            await ExecuteHandler();
        }
        catch (Exception ex)
        {
            ExceptionDispatchInfo.Capture(ex).Throw();
        }
        finally
        {
            IsExecuting = false;
            RaiseCanExecuteChanged();
        }
    }

    public void RaiseCanExecuteChanged()
    {
        Dispatcher.UIThread.InvokeAsync(
            () => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        Dispatcher.UIThread.InvokeAsync(() => PropertyChanged?.Invoke(
            this, new PropertyChangedEventArgs(propertyName)));
    }
}