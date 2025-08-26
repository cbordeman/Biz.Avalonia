using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Biz.Shell;

public static class TaskExtensions
{
    public static void LogException(this Task task, string taskDescription, ILogger? logger = null)
    {
        try
        {
            logger ??= ContainerLocator.Container.Resolve<ILogger<object>>();
        }
        catch (Exception e)
        {
            // We'll fall back to Debug logging.
        }

        try
        {
            task.WaitAsync(TimeSpan.FromSeconds(30)).ContinueWith(
                t =>
                {
                    if (t.IsFaulted)
                    {
                        t.Exception?.Handle((Exception e) =>
                        {
                            if (e is TimeoutException or OperationCanceledException)
                            {
                                if (logger != null)
                                    logger.LogError(e,
                                        "Task timed out.  " +
                                        "Task Description: {TaskDescription}, " +
                                        "Exception type: {ExceptionType}",
                                        taskDescription, e.GetType().Name);
                                else
                                    Debug.WriteLine($"Task timed out: {taskDescription}");
                            }
                            else if (e is OperationCanceledException)
                            {
                                if (logger != null)
                                    logger.LogInformation(
                                        "Operation cancelled.  " +
                                        "Task Description: {TaskDescription}, " +
                                        "Exception type: {ExceptionType}",
                                        taskDescription, e.GetType().Name);
                                else
                                    Debug.WriteLine($"Operation cancelled: {taskDescription}");
                            }
                            else
                            {
                                if (logger != null)
                                    logger.LogError(e,
                                        "Exception thrown during task.  " +
                                        "Task description: {TaskDescription}: {Message}",
                                        taskDescription, e.Message);
                                else
                                    Debug.WriteLine($"Task threw an exception: {taskDescription}.  Exception: ({e.GetType().Name}) {e}");
                            }
                            return true;
                        });
                    }
                });
        }
        catch (Exception e)
        {
            // shouldn't get here.
        }
    }
}
