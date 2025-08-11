namespace Biz.Core;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PageAttribute(string areaName) : Attribute
{
    /// <summary>
    /// Area the Sidebar will use to highlight which part of the
    /// application is selected.
    /// </summary>
    public string AreaName { get; } = areaName;
}