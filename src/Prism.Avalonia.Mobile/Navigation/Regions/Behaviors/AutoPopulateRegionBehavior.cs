using Prism.Ioc;

namespace Prism.Navigation.Regions.Behaviors;

public class AutoPopulateRegionBehavior : RegionBehavior
{
    public const string BehaviorKey = "AutoPopulate";

    protected override void OnAttach()
    {
        if (string.IsNullOrEmpty(Region.Name)) return;

        var c = ContainerLocator.Container;
        var viewRegistry = c.Resolve<IRegionViewRegistry>();
        var contents = viewRegistry.GetContents(Region.Name, c);
        foreach (var view in contents)
        {
            if (view is not null) Region.Add(view);
        }
    }
}
