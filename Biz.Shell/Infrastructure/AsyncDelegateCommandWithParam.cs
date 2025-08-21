namespace Biz.Shell.Infrastructure;

public class AsyncDelegateCommandWithParam<T> : AsyncDelegateCommand<T>
{
    public AsyncDelegateCommandWithParam(
        Func<T, Task> executeMethod, 
        Func<T, bool> canExecuteMethod)
        : base(executeMethod: (p, c) => 
            executeMethod(p).WaitAsync(c), canExecuteMethod)
    {
    }
    
    public AsyncDelegateCommandWithParam(
        Func<T, Task> executeMethod)
        : base(executeMethod: (p, c) => 
            executeMethod(p).WaitAsync(c))
    {
    }
}
