using Avalonia.Controls.Primitives;
using ShadUI;
using Avalonia.VisualTree; // for GetVisualChildren extension

namespace Biz.Shell;

public class SidebarItemNoSelect : SidebarItem
{
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        // Use the template's NameScope
        var panel = FindVisualChildByName<Border>(this, "SelectedBackground");
        if (panel != null)
            panel.IsVisible = false;

        //var selectedBackgroundBorder = this.Get<SidebarItem>("SelectedBackground");
        //if (selectedBackgroundBorder != null)
        //    selectedBackgroundBorder.IsVisible = false; // or Visibility = Visibility.Collapsed;
    }
    
     

    private T? FindVisualChildByName<T>(Visual parent, string name) 
        where T : Visual
    {
        foreach (var child in parent.GetVisualChildren())
        {
            if (child is T tChild && child.Name == name)
                return tChild;

            var result = FindVisualChildByName<T>(child, name);
            if (result != null)
                return result;
        }
        return null;
    }
}
