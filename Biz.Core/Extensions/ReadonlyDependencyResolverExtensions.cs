using CompositeFramework.Core.Extensions;

namespace Biz.Core.Extensions;

public static class ReadonlyDependencyResolverExtensions
{
    /// <summary>
    /// A safe version of Splat's GetServie<T>() method.
    /// Instead of returning null, throws an exception.
    /// Attempts to register classes if not views.
    /// </summary>
    /// <param name="resolver"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="TypeResolutionFailedException"></exception>
    public static T Resolve<T>(this IReadonlyDependencyResolver resolver)
    {
        try
        {
            var service = resolver.GetService<T>();
            if (service is null &&
                typeof(T).IsClass) //&&
                //!typeof(T).Name.IsView())
            {
                Locator.CurrentMutable.Register(() => Locator.Current.GetService<T>());
            }
            if (service is null)
                throw new TypeResolutionFailedException(typeof(T), null);
            return service;
        }
        catch (Exception? e)
        {
            throw new TypeResolutionFailedException(typeof(T), e);
        }
    }
}