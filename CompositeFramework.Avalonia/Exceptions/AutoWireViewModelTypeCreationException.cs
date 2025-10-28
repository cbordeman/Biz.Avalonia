namespace CompositeFramework.Avalonia.Exceptions;

public class AutoWireViewModelTypeCreationException : Exception
{
    public Type ViewType { get; }
    public string ViewModelType { get; }
    
    public AutoWireViewModelTypeCreationException(Type viewType,
        string viewModelType, Exception? innerException)
        : base($"For view {viewType.AssemblyQualifiedName}, could not create an " +
               $"instance of viewmodel type {viewModelType}.", 
            innerException)
    {
        ViewType = viewType;
        ViewModelType = viewModelType;
    }
}
