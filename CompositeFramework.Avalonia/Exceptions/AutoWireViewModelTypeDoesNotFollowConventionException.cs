namespace CompositeFramework.Avalonia.Exceptions;

public class AutoWireViewModelTypeDoesNotFollowConventionException
    : Exception
{
    public AutoWireViewModelTypeDoesNotFollowConventionException(
        string viewName)
        : base($"View type \"{viewName}\" does not follow convention.  " +
               $"The type namespace should contain \".Views.\", or " +
               $"the type name should end or one of the suffixes " +
               $"configured via ViewModelLocator.Configure() such as" +
               $"\"View\", \"UserControl\", \"Page\", or \"Dialog\".")
    { }
}
