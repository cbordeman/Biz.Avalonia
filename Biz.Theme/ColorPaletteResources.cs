using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Biz.Theme.Accents;

namespace Avalonia.Themes.Fluent;

/// <summary>
/// Represents a specialized resource dictionary that contains color resources used by FluentTheme elements.
/// </summary>
/// <remarks>
/// This class can only be used in <see cref="FluentTheme.Palettes"/>.
/// </remarks>
public partial class ColorPaletteResources : ResourceProvider
{
    private readonly Dictionary<string, Color> colors = new(StringComparer.InvariantCulture);

    public override bool HasResources => hasAccentColor || colors.Count > 0;

    public override bool TryGetResource(object key, ThemeVariant? theme, out object? value)
    {
        if (key is string strKey)
        {
            if (strKey.Equals(SystemAccentColors.AccentKey, StringComparison.InvariantCulture))
            {
                value = accentColor;
                return hasAccentColor;
            }

            if (strKey.Equals(SystemAccentColors.AccentDark1Key, StringComparison.InvariantCulture))
            {
                value = accentColorDark1;
                return hasAccentColor;
            }

            if (strKey.Equals(SystemAccentColors.AccentDark2Key, StringComparison.InvariantCulture))
            {
                value = accentColorDark2;
                return hasAccentColor;
            }

            if (strKey.Equals(SystemAccentColors.AccentDark3Key, StringComparison.InvariantCulture))
            {
                value = accentColorDark3;
                return hasAccentColor;
            }

            if (strKey.Equals(SystemAccentColors.AccentLight1Key, StringComparison.InvariantCulture))
            {
                value = accentColorLight1;
                return hasAccentColor;
            }

            if (strKey.Equals(SystemAccentColors.AccentLight2Key, StringComparison.InvariantCulture))
            {
                value = accentColorLight2;
                return hasAccentColor;
            }

            if (strKey.Equals(SystemAccentColors.AccentLight3Key, StringComparison.InvariantCulture))
            {
                value = accentColorLight3;
                return hasAccentColor;
            }

            if (colors.TryGetValue(strKey, out var color))
            {
                value = color;
                return true;
            }
        }

        value = null;
        return false;
    }

    private Color GetColor(string key)
    {
        if (colors.TryGetValue(key, out var color))
        {
            return color;
        }

        return default;
    }

    private void SetColor(string key, Color value)
    {
        if (value == default)
        {
            colors.Remove(key);
        }
        else
        {
            colors[key] = value;
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AccentProperty)
        {
            hasAccentColor = accentColor != default;

            if (hasAccentColor)
            {
                (accentColorDark1, accentColorDark2, accentColorDark3,
                        accentColorLight1, accentColorLight2, accentColorLight3) =
                    SystemAccentColors.CalculateAccentShades(accentColor);
            }
            RaiseResourcesChanged();
        }
    }
}
