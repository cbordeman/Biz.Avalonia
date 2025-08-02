using System;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Styling;

namespace Biz.Theme.Accents;

internal sealed class SystemAccentColors : ResourceProvider
{
    public const string AccentKey = "SystemAccentColor";
    public const string AccentDark1Key = "SystemAccentColorDark1";
    public const string AccentDark2Key = "SystemAccentColorDark2";
    public const string AccentDark3Key = "SystemAccentColorDark3";
    public const string AccentLight1Key = "SystemAccentColorLight1";
    public const string AccentLight2Key = "SystemAccentColorLight2";
    public const string AccentLight3Key = "SystemAccentColorLight3";
    
    private static readonly Color SDefaultSystemAccentColor = Color.FromRgb(0, 120, 215);
    private bool invalidateColors = true;
    private Color systemAccentColor;
    private Color systemAccentColorDark1, systemAccentColorDark2, systemAccentColorDark3;
    private Color systemAccentColorLight1, systemAccentColorLight2, systemAccentColorLight3;

    public override bool HasResources => true;
    public override bool TryGetResource(object key, ThemeVariant theme, out object value)
    {
        if (key is string strKey)
        {
            if (strKey.Equals(AccentKey, StringComparison.InvariantCulture))
            {
                EnsureColors();
                value = systemAccentColor;
                return true;
            }

            if (strKey.Equals(AccentDark1Key, StringComparison.InvariantCulture))
            {
                EnsureColors();
                value = systemAccentColorDark1;
                return true;
            }

            if (strKey.Equals(AccentDark2Key, StringComparison.InvariantCulture))
            {
                EnsureColors();
                value = systemAccentColorDark2;
                return true;
            }

            if (strKey.Equals(AccentDark3Key, StringComparison.InvariantCulture))
            {
                EnsureColors();
                value = systemAccentColorDark3;
                return true;
            }

            if (strKey.Equals(AccentLight1Key, StringComparison.InvariantCulture))
            {
                EnsureColors();
                value = systemAccentColorLight1;
                return true;
            }

            if (strKey.Equals(AccentLight2Key, StringComparison.InvariantCulture))
            {
                EnsureColors();
                value = systemAccentColorLight2;
                return true;
            }

            if (strKey.Equals(AccentLight3Key, StringComparison.InvariantCulture))
            {
                EnsureColors();
                value = systemAccentColorLight3;
                return true;
            }
        }

        value = null;
        return false;
    }

    protected override void OnAddOwner(IResourceHost owner)
    {
        if (GetFromOwner(owner) is { } platformSettings)
        {
            platformSettings.ColorValuesChanged += PlatformSettingsOnColorValuesChanged;
        }

        invalidateColors = true;
    }

    protected override void OnRemoveOwner(IResourceHost owner)
    {
        if (GetFromOwner(owner) is { } platformSettings)
        {
            platformSettings.ColorValuesChanged -= PlatformSettingsOnColorValuesChanged;
        }

        invalidateColors = true;
    }

    private void EnsureColors()
    {
        if (invalidateColors)
        {
            invalidateColors = false;

            var platformSettings = GetFromOwner(Owner);
            
            systemAccentColor = platformSettings?.GetColorValues().AccentColor1 ?? SDefaultSystemAccentColor;
            (systemAccentColorDark1,systemAccentColorDark2, systemAccentColorDark3,
                    systemAccentColorLight1, systemAccentColorLight2, systemAccentColorLight3) = CalculateAccentShades(systemAccentColor);
        }
    }

    private static IPlatformSettings GetFromOwner(IResourceHost owner)
    {
        return owner switch
        {
            Application app => app.PlatformSettings,
            Visual visual => TopLevel.GetTopLevel(visual)?.PlatformSettings,
            _ => null
        };
    }
    
    public static (Color d1, Color d2, Color d3, Color l1, Color l2, Color l3) CalculateAccentShades(Color accentColor)
    {
        // dark1step = (hslAccent.L - SystemAccentColorDark1.L) * 255
        const double dark1Step = 28.5 / 255d;
        const double dark2Step = 49 / 255d;
        const double dark3Step = 74.5 / 255d;
        // light1step = (SystemAccentColorLight1.L - hslAccent.L) * 255
        const double light1Step = 39 / 255d;
        const double light2Step = 70 / 255d;
        const double light3Step = 103 / 255d;
        
        var hslAccent = accentColor.ToHsl();

        return (
            // Darker shades
            new HslColor(hslAccent.A, hslAccent.H, hslAccent.S, hslAccent.L - dark1Step).ToRgb(),
            new HslColor(hslAccent.A, hslAccent.H, hslAccent.S, hslAccent.L - dark2Step).ToRgb(),
            new HslColor(hslAccent.A, hslAccent.H, hslAccent.S, hslAccent.L - dark3Step).ToRgb(),

            // Lighter shades
            new HslColor(hslAccent.A, hslAccent.H, hslAccent.S, hslAccent.L + light1Step).ToRgb(),
            new HslColor(hslAccent.A, hslAccent.H, hslAccent.S, hslAccent.L + light2Step).ToRgb(),
            new HslColor(hslAccent.A, hslAccent.H, hslAccent.S, hslAccent.L + light3Step).ToRgb()
        );
    }
    
    private void PlatformSettingsOnColorValuesChanged(object sender, PlatformColorValues e)
    {
        invalidateColors = true;
        Owner?.NotifyHostedResourcesChanged(ResourcesChangedEventArgs.Empty);
    }
}
