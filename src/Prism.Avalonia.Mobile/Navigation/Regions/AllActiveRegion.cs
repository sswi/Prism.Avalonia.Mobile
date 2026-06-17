namespace Prism.Navigation.Regions;

/// <summary>
/// A region where all added views are always active.
/// <see cref="Deactivate"/> is not supported.
/// Used by <see cref="Adapters.ItemsControlRegionAdapter"/>.
/// </summary>
public class AllActiveRegion : Region
{
    /// <inheritdoc />
    public override IViewsCollection ActiveViews => Views;

    /// <inheritdoc />
    public override void Deactivate(object view)
    {
        throw new InvalidOperationException("Cannot deactivate views in an AllActiveRegion.");
    }
}
