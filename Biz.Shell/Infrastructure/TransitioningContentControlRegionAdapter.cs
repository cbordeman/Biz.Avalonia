using Avalonia.Controls;

namespace Biz.Shell.Infrastructure;

public class TransitioningContentControlRegionAdapter : RegionAdapterBase<TransitioningContentControl>
{
    public TransitioningContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
        : base(regionBehaviorFactory)
    {
    }

    protected override void Adapt(IRegion region, TransitioningContentControl regionTarget)
    {
        region.Views.CollectionChanged += (s, e) =>
        {
            // Show the *single active view* as content:
            if (region.Views.Any())
                regionTarget.Content = region.Views.First();
            else
                regionTarget.Content = null;
        };
    }

    protected override IRegion CreateRegion()
    {
        // Use SingleActiveRegion to ensure only one view is visible at a time.
        return new SingleActiveRegion();
    }
}