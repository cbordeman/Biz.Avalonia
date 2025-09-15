namespace CompositeFramework.Core;

/// <summary>
/// Single subscriber / publisher mechanism.
/// </summary>
/// <typeparam name="T"></typeparam>
public class RequestObject<T>
{
    Func<T?, Task> Callback { get; }
    
    public RequestObject(Func<T?, Task> callback)
    {
        ArgumentChecker.ThrowIfNull(callback);
        Callback = callback;
    }
    
    public Task Invoke(T? value)
    {
        return Callback(value);
    }
}