namespace Prism.Navigation.Regions;

/// <summary>
/// A region that allows at most one active view at a time.
/// When a new view is activated, the previously active view is automatically deactivated.
/// Used by <see cref="Adapters.ContentControlRegionAdapter"/>.
/// </summary>
public class SingleActiveRegion : Region
{
    /// <inheritdoc />
    public override void Activate(object view)
    {
        // Deactivate the currently active view first
        var currentActive = ActiveViews.FirstOrDefault();
        if (currentActive is not null && currentActive != view)
        {
            base.Deactivate(currentActive);
        }

        base.Activate(view);
    }
}
