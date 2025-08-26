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
            // I guess we don't get to log.
        }

        try
        {
            task.Wait(TimeSpan.FromSeconds(30));
        }
        catch (TimeoutException e)
        {
            if (logger != null)
                logger.LogInformation(
                    "Task timed out.  " +
                    "Task Description: {TaskDescription}, " +
                    "Exception type: {ExceptionType}",
                    taskDescription, e.GetType().Name);
            else
                Debug.WriteLine($"Task timed out: {taskDescription}");
            return;
        }
        catch (Exception e)
        {
            if (logger != null)
                logger.LogError(e,
                    "Exception thrown during task.  " +
                    "Task description: {TaskDescription}: {Message}",
                    taskDescription, e.Message);
            else
                Debug.WriteLine($"Task threw an exception: {taskDescription}.  Exception: ({e.GetType().Name}) {e}");
            return;
        }

        // Exception is eaten.
    }
}