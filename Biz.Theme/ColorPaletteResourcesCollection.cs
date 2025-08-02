using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Biz.Theme;

internal sealed class ColorPaletteResourcesCollection : ResourceProvider, IDictionary<ThemeVariant, ColorPaletteResources>
{
    private readonly AvaloniaDictionary<ThemeVariant, ColorPaletteResources> inner;

    public ColorPaletteResourcesCollection()
    {
        inner = new AvaloniaDictionary<ThemeVariant, ColorPaletteResources>(2);
        inner.ForEachItem(
            (key, x) =>
            {
                if (Owner is not null)
                {
                    ((IResourceProvider)x).AddOwner(Owner);
                }

                if (key != ThemeVariant.Dark && key != ThemeVariant.Light)
                {
                    throw new InvalidOperationException(
                        $"{nameof(BizTheme)}.{nameof(BizTheme.Palettes)} only supports Light and Dark variants.");
                }
            },
            (_, x) =>
            {
                if (Owner is not null)
                {
                    ((IResourceProvider)x).RemoveOwner(Owner);
                }
            },
            () => throw new NotSupportedException("Dictionary reset not supported"));
    }

    public override bool HasResources => inner.Count > 0;
    public override bool TryGetResource(object key, ThemeVariant theme, out object value)
    {
        if (theme == null || theme == ThemeVariant.Default)
        {
            theme = ThemeVariant.Light;
        }

        if (inner.TryGetValue(theme, out var themePaletteResources)
            && themePaletteResources.TryGetResource(key, theme, out value))
        {
            return true;
        }

        value = null;
        return false;
    }

    protected override void OnAddOwner(IResourceHost owner)
    {
        base.OnAddOwner(owner);
        foreach (var palette in inner.Values)
        {
            ((IResourceProvider)palette).AddOwner(owner);
        }
    }

    protected override void OnRemoveOwner(IResourceHost owner)
    {
        base.OnRemoveOwner(owner);
        foreach (var palette in inner.Values)
        {
            ((IResourceProvider)palette).RemoveOwner(owner);
        }
    }

    IEnumerator<KeyValuePair<ThemeVariant, ColorPaletteResources>> IEnumerable<KeyValuePair<ThemeVariant, ColorPaletteResources>>.GetEnumerator()
    {
        return inner.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)inner).GetEnumerator();
    }

    void ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>.Add(KeyValuePair<ThemeVariant, ColorPaletteResources> item)
    {
        ((ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>)inner).Add(item);
    }

    void ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>.Clear()
    {
        inner.Clear();
    }

    bool ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>.Contains(KeyValuePair<ThemeVariant, ColorPaletteResources> item)
    {
        return ((ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>)inner).Contains(item);
    }

    void ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>.CopyTo(KeyValuePair<ThemeVariant, ColorPaletteResources>[] array, int arrayIndex)
    {
        inner.CopyTo(array, arrayIndex);
    }

    bool ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>.Remove(KeyValuePair<ThemeVariant, ColorPaletteResources> item)
    {
        return ((ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>)inner).Remove(item);
    }

    int ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>.Count => inner.Count;

    bool ICollection<KeyValuePair<ThemeVariant, ColorPaletteResources>>.IsReadOnly => inner.IsReadOnly;

    void IDictionary<ThemeVariant, ColorPaletteResources>.Add(ThemeVariant key, ColorPaletteResources value)
    {
        inner.Add(key, value);
    }

    bool IDictionary<ThemeVariant, ColorPaletteResources>.ContainsKey(ThemeVariant key)
    {
        return inner.ContainsKey(key);
    }

    bool IDictionary<ThemeVariant, ColorPaletteResources>.Remove(ThemeVariant key)
    {
        return inner.Remove(key);
    }

    bool IDictionary<ThemeVariant, ColorPaletteResources>.TryGetValue(ThemeVariant key,
#if NET6_0_OR_GREATER
        [MaybeNullWhen(false)]
#endif
        out ColorPaletteResources value)
    {
        return inner.TryGetValue(key, out value);
    }

    ColorPaletteResources IDictionary<ThemeVariant, ColorPaletteResources>.this[ThemeVariant key]
    {
        get => inner[key];
        set => inner[key] = value;
    }

    ICollection<ThemeVariant> IDictionary<ThemeVariant, ColorPaletteResources>.Keys => inner.Keys;

    ICollection<ColorPaletteResources> IDictionary<ThemeVariant, ColorPaletteResources>.Values => inner.Values;
}
