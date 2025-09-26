using Avalonia.Controls;

namespace CompositeFramework.Avalonia.Exceptions;
 
public class DuplicateSectionNameException(Control control,
    string duplicateSectionName) 
    : Exception($"Duplicate section name " +
                $"\"{duplicateSectionName}\"on Control " +
                $"{control.GetType().Name}.")
{
    public Control Control { get; } = control;
    public string DuplicateSectionName { get; } = duplicateSectionName;
}
