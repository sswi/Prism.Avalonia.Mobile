namespace Prism.Navigation.Regions.Behaviors;

/// <summary>
/// Sets <see cref="IActiveAware.IsActive"/> on views when they are added to
/// or removed from the region's active views collection.
/// </summary>
public class RegionActiveAwareBehavior : RegionBehavior
{
    /// <summary>
    /// The behavior key.
    /// </summary>
    public const string BehaviorKey = "ActiveAware";

    /// <inheritdoc />
    protected override void OnAttach()
    {
        Region.ActiveViews.CollectionChanged += OnActiveViewsChanged;
    }

    private void OnActiveViewsChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add && e.NewItems is not null)
        {
            foreach (var item in e.NewItems)
            {
                InvokeActiveAware(item, true);
            }
        }
        else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove && e.OldItems is not null)
        {
            foreach (var item in e.OldItems)
            {
                InvokeActiveAware(item, false);
            }
        }
    }

    private static void InvokeActiveAware(object? item, bool isActive)
    {
        if (item is IActiveAware activeAware)
            activeAware.IsActive = isActive;

        if (item is Avalonia.StyledElement styled && styled.DataContext is IActiveAware vmActiveAware)
            vmActiveAware.IsActive = isActive;
    }
}
