using Avalonia;

namespace Prism.Navigation.Regions.Behaviors;

public class RegionManagerRegistrationBehavior : RegionBehavior
{
    public const string BehaviorKey = "RegionManagerRegistration";

    protected override void OnAttach()
    {
        if (Region is not ITargetAwareRegion ta || ta.TargetElement is null) return;

        var p = (ta.TargetElement as Visual)?.Parent;
        while (p is not null)
        {
            var rm = RegionManager.GetRegionManager(p);
            if (rm is not null)
            {
                if (!rm.Regions.ContainsRegionWithName(Region.Name))
                    rm.Regions.Add(Region);
                return;
            }
            p = (p as Visual)?.Parent;
        }
    }
}
