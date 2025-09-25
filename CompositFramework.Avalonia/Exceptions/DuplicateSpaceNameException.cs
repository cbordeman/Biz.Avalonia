using Avalonia.Controls;

namespace CompositFramework.Avalonia.Exceptions;

public class DuplicateSpaceNameException(Control control,
    string duplicateSpaceName) 
    : Exception($"Duplicate space name " +
                $"\"{duplicateSpaceName}\"on Control " +
                $"{control.GetType().Name}.",
    innerException)
{
    public Control Control { get; } = control;
    public string DuplicateSpaceName { get; } = duplicateSpaceName;
}
