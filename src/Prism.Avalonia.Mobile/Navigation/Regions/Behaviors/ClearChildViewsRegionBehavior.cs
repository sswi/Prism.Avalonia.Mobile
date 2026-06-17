using Avalonia;

namespace Prism.Navigation.Regions.Behaviors;

public class ClearChildViewsRegionBehavior : RegionBehavior
{
    public const string BehaviorKey = "ClearChildViews";

    protected override void OnAttach()
    {
        if (Region is ITargetAwareRegion ta && ta.TargetElement is Visual v)
        {
            // Use Avalonia 12's visual tree detach event
            v.DetachedFromVisualTree += (s, e) => Region.RemoveAll();
        }
    }
}
