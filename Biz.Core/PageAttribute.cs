using System;

namespace Biz.Core;

/// <summary>
/// Area the sidebar will look for at the beginning of the
/// current page's uri in order to highlight the correct item.
/// Probably best to use the module name.
/// </summary>
/// <param name="areaName"></param>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class AreaAttribute(string areaName) : Attribute
{
    public string AreaName { get; } = areaName;
}