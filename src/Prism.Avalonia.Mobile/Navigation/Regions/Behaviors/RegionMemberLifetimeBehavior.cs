namespace Prism.Navigation.Regions.Behaviors;

/// <summary>
/// Removes deactivated views from the region unless they implement
/// <see cref="IRegionMemberLifetime"/> with <c>KeepAlive = true</c>.
/// </summary>
public class RegionMemberLifetimeBehavior : RegionBehavior
{
    /// <summary>
    /// The behavior key.
    /// </summary>
    public const string BehaviorKey = "RegionMemberLifetime";

    /// <inheritdoc />
    protected override void OnAttach()
    {
        Region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
    }

    private void OnActiveViewsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                if (!ShouldKeepAlive(item))
                {
                    Region.Remove(item);
                }
            }
        }
    }

    private static bool ShouldKeepAlive(object? item)
    {
        if (item is IRegionMemberLifetime lifetime)
            return lifetime.KeepAlive;

        if (item is Avalonia.StyledElement styled && styled.DataContext is IRegionMemberLifetime vm)
            return vm.KeepAlive;

        return false;
    }
}
