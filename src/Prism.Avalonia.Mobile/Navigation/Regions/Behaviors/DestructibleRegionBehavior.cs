namespace Prism.Navigation.Regions.Behaviors;

public class DestructibleRegionBehavior : RegionBehavior
{
    public const string BehaviorKey = "Destructible";

    protected override void OnAttach()
    {
        Region.Views.CollectionChanged += (s, e) =>
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems is not null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is IDestructible d) d.Destroy();
                }
            }
        };
    }
}
