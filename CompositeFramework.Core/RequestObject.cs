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
        ArgumentNullException.ThrowIfNull(handler);

        ImmutableInterlocked.Update(
            ref subscribers,
            s => s.Add(handler));
    }

    public void Unsubscribe(Func<Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        // Atomically update the subscribers list
        ImmutableInterlocked.Update(
            ref subscribers,
            s => s.Remove(handler));

        // Note: Remove will have no effect if handler not found
    }


    /// <summary>
    /// Invokes all handlers simultaneously.
    /// </summary>
    /// <exception cref="AggregateException"></exception>
    public Task PublishParallelAsync()
    {
        var snapshot = subscribers;
        return Task.WhenAll(snapshot.Select(h => h()));
    }
    
    /// <summary>
    /// Publish one at a time, awaiting each.
    /// </summary>
    /// <exception cref="AggregateException"></exception>
    public async Task PublishSequentiallyAsync()
    {
        var snapshot = subscribers;
        var len = snapshot.Length;
        List<Exception>? exceptions = null;
        foreach (var handler in snapshot)
            try
            {
                await handler().ConfigureAwait(false);
            }
            catch (Exception e)
            {
                exceptions ??= new List<Exception>(len);
                exceptions.Add(e);
            }
        if (exceptions != null)
            throw new AggregateException(exceptions);
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
        ArgumentNullException.ThrowIfNull(handler);

        ImmutableInterlocked.Update(
            ref subscribers,
            s => s.Add(handler));
    }

    public void Unsubscribe(Func<T, Task> handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        ImmutableInterlocked.Update(
            ref subscribers,
            s => s.Remove(handler));
    }

    /// <summary>
    /// Invokes all handlers concurrently.
    /// </summary>
    /// <exception cref="AggregateException"></exception>
    public Task PublishParallelAsync(T arg)
    {
        var snapshot = subscribers;
        return Task.WhenAll(snapshot.Select(h => h(arg)));
    }

    /// <summary>
    /// Invokes handlers one at a time, sequentially awaiting each.
    /// </summary>
    /// <exception cref="AggregateException"></exception>
    public async Task PublishSequentiallyAsync(T arg)
    {
        var snapshot = subscribers;
        var len = snapshot.Length;
        List<Exception>? exceptions = null;
        foreach (var handler in snapshot)
            try
            {
                await handler(arg).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                exceptions ??= new List<Exception>(len);
                exceptions.Add(e);
            }
        if (exceptions != null)
            throw new AggregateException(exceptions);
    }
}