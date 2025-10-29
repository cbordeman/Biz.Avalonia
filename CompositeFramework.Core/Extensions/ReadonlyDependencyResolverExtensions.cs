using Splat;

namespace CompositeFramework.Core.Extensions;

public static class ReadonlyDependencyResolverExtensions
{
    /// <summary>
    /// A safe version of Splat's GetServie{T}() method.
    /// Instead of returning null, throws an exception.
    /// </summary>
    /// <param name="resolver"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    /// <exception cref="TypeResolutionFailedException"></exception>
    public static object Resolve(
        this IReadonlyDependencyResolver resolver,
        Type type)
    {
        try
        {
            var service = resolver.GetService(type);
            if (service is null)
                throw new TypeResolutionFailedException(
                    resolver, type, null);
            return service;
        }
        catch (Exception? e)
        {
            throw new TypeResolutionFailedException(
                resolver, type, e);
        }
    }
    
    /// <summary>
    /// A safe version of Splat's GetServie{T}() method.
    /// Instead of returning null, throws an exception.
    /// </summary>
    /// <param name="resolver"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    /// <exception cref="TypeResolutionFailedException"></exception>
    public static T Resolve<T>(
        this IReadonlyDependencyResolver resolver)
    {
        try
        {
            var service = resolver.GetService<T>();
            if (service is null)
                throw new TypeResolutionFailedException(
                    resolver, typeof(T), null);
            return service;
        }
        catch (Exception? e)
        {
            throw new TypeResolutionFailedException(
                resolver, typeof(T), e);
        }
    }
}