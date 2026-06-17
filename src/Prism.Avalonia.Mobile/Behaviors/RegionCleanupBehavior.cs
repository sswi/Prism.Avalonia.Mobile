using Avalonia.Controls;

namespace Prism.Behaviors;

/// <summary>
/// Placeholder for Phase 3 region cleanup behavior.
/// When a page is detached from the visual tree, this behavior will
/// clear any child regions associated with the page.
/// </summary>
internal sealed class RegionCleanupBehavior
{
    public void Apply(Page page)
    {
        page.DetachedFromVisualTree += (s, e) =>
        {
            // Phase 3: clear child regions
            // RegionManager.ClearChildRegions(page);
        };
    }
}
