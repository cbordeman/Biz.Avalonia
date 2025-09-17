using System.Collections.Immutable;

namespace CompositeFramework.Core;

/// <summary>
/// A thread-safe pub/sub mechanism that supports async publish. 
/// </summary>
public sealed class AsyncEvent
{
    // Immutable array reference storing subscribers
    private ImmutableArray<Func<Task>> subscribers = 
        ImmutableArray<Func<Task>>.Empty;

    public void Subscribe(Func<Task> handler)
    {
        ArgumentChecker.ThrowIfNull(handler);

        ImmutableArray<Func<Task>> original, updated;
        do
        {
            original = subscribers;
            updated = original.Add(handler);
            // Atomically swap if _subscribers is still original
        } while (Interlocked.CompareExchange(
                     ref subscribers, updated, original) != original);
    }

    public void Unsubscribe(Func<Task> handler)
    {
        ArgumentChecker.ThrowIfNull(handler);

        ImmutableArray<Func<Task>> original, updated;
        do
        {
            original = subscribers;
            updated = original.Remove(handler);
            if (updated == original)
                // handler not found, break early
                break;
            // Atomically swap if subscribers is still original
        } while (Interlocked.CompareExchange(
                     ref subscribers, updated, original) != original);
    }

    public async Task PublishAsync()
    {
        var snapshot = subscribers;
        foreach (var handler in snapshot)
            await handler().ConfigureAwait(false);
    }
}

/// <summary>
/// A thread-safe pub/sub mechanism that supports async publish. 
/// </summary>
public sealed class AsyncEvent<T>
{
    // Immutable array reference storing subscribers
    private ImmutableArray<Func<T, Task>> subscribers = 
        ImmutableArray<Func<T, Task>>.Empty;

    public void Subscribe(Func<T, Task> handler)
    {
        ArgumentChecker.ThrowIfNull(handler);

        ImmutableArray<Func<T, Task>> original, updated;
        do
        {
            original = subscribers;
            updated = original.Add(handler);
            // Atomically swap if _subscribers is still original
        } while (Interlocked.CompareExchange(
                     ref subscribers, updated, original) != original);
    }

    public void Unsubscribe(Func<T, Task> handler)
    {
        ArgumentChecker.ThrowIfNull(handler);

        ImmutableArray<Func<T, Task>> original, updated;
        do
        {
            original = subscribers;
            updated = original.Remove(handler);
            if (updated == original)
                // handler not found, break early
                break;
            // Atomically swap if subscribers is still original
        } while (Interlocked.CompareExchange(
                     ref subscribers, updated, original) != original);
    }

    public async Task PublishAsync(T arg)
    {
        var snapshot = subscribers;
        foreach (var handler in snapshot)
            await handler(arg).ConfigureAwait(false);
    }
}

