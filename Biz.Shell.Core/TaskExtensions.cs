using System.Diagnostics;

namespace Biz.Shell.Core;

public static class TaskExtensions
{
    public static Task LogException(this Task task)
    {
        if (task.IsFaulted)
            Debug.WriteLine($"Exception thrown: {task.Exception.GetBaseException()}");
        return Task.CompletedTask;

        // Exception is eaten.
    }
}