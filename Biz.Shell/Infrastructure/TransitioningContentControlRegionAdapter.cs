namespace Biz.Shell.Infrastructure;

public class TransitioningContentControlRegionAdapter : ContentControlRegionAdapter
{
    public TransitioningContentControlRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory) : 
        base(regionBehaviorFactory)
    {
    }
}