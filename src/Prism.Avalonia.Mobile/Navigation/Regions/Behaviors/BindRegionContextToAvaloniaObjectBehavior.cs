namespace Prism.Navigation.Regions.Behaviors;

public class BindRegionContextToAvaloniaObjectBehavior : RegionBehavior
{
    public const string BehaviorKey = "ContextToAvaloniaObject";

    protected override void OnAttach()
    {
        Region.Views.CollectionChanged += (s, e) =>
        {
            if (Region.Context is not null)
            {
                foreach (var view in Region.Views)
                {
                    if (view is Avalonia.AvaloniaObject ao)
                        RegionContext.SetRegionContextValue(ao, Region.Context);
                }
            }
        };
    }
}
