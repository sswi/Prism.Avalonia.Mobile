namespace Prism.Navigation.Regions.Behaviors;

/// <summary>
/// Two-way sync between the region's <c>Context</c> property and the host
/// control's <c>RegionContext</c> attached property.
/// </summary>
public class SyncRegionContextWithHostBehavior : RegionBehavior
{
    /// <summary>
    /// The behavior key.
    /// </summary>
    public const string BehaviorKey = "SyncRegionContextWithHost";

    /// <inheritdoc />
    protected override void OnAttach()
    {
        if (Region is not ITargetAwareRegion targetAware || targetAware.TargetElement is null)
            return;

        // Forward existing context from host to region
        var hostContext = RegionContext.GetRegionContextValue(targetAware.TargetElement);
        if (hostContext is not null)
            Region.Context = hostContext;

        // TODO: set up two-way binding
    }
}
