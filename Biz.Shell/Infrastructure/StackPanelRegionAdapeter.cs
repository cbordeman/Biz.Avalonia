using System.Collections.Specialized;

namespace Biz.Shell.Infrastructure;

public class StackPanelRegionAdapter : RegionAdapterBase<StackPanel>
{
    public StackPanelRegionAdapter(IRegionBehaviorFactory factory)
        : base(factory)
    {
    }

    protected override void Adapt(IRegion region, StackPanel regionTarget)
    {
        region.Views.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var view in e.NewItems!) 
                    regionTarget.Children.Add((Control)view);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var view in e.OldItems!) 
                    regionTarget.Children.Remove((Control)view);
            }
        };
    }

    protected override IRegion CreateRegion()
    {
        return new AllActiveRegion();
    }
}