namespace Biz.Core;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class PageAttribute(string areaName) : Attribute
{
    /// <summary>
    /// Area the Sidebar will use to highlight which part of the
    /// application is selected.  Must appear at the beginning of the
    /// Uri to the current view before the slash, if there is one.
    /// </summary>
    public string AreaName { get; } = areaName;
}