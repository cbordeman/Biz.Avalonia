namespace CompositeFramework.Avalonia.Exceptions;

public class AutoWireViewModelTypeCreationException : Exception
{
    public string ViewModelType { get; }
    
    public AutoWireViewModelTypeCreationException(string viewType,
        string viewModelType, Exception? innerException)
        : base($"For view {viewType}, could not create an " +
               $"instance of viewmodel type {viewModelType}.", 
            innerException)
    {
        ViewModelType = viewModelType;
    }
}
