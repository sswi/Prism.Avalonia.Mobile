using Avalonia.Controls;

namespace Prism.Behaviors;

/// <summary>
/// Default implementation of <see cref="IPageBehaviorFactory"/>.
/// Applies the standard set of Prism page behaviors.
/// </summary>
/// <remarks>
/// Behaviors applied (order matters):
/// <list type="number">
///   <item><c>PageScopeBehavior</c> — stores DI scope on the page</item>
///   <item><c>PageLifeCycleAwareBehavior</c> — appearing/disappearing events</item>
///   <item><c>PageActiveAwareBehavior</c> — per-page active/inactive tracking</item>
///   <item><c>NavigationPageActiveAwareBehavior</c> — push/pop-based activation</item>
///   <item><c>PageSystemBackBehavior</c> — device back button interception</item>
///   <item><c>TabbedPageActiveAwareBehavior</c> — tab selection-based activation</item>
///   <item><c>RegionCleanupBehavior</c> — Phase 3 region cleanup</item>
/// </list>
/// </remarks>
public class DefaultPageBehaviorFactory : IPageBehaviorFactory
{
    /// <inheritdoc />
    public void ApplyPageBehaviors(Page page)
    {
        // Common behaviors applied to ALL page types
        var scopeBehavior = new PageScopeBehavior();
        // scope is applied by NavigationRegistry

        new PageLifeCycleAwareBehavior().Apply(page);
        new PageActiveAwareBehavior().Apply(page);

        // NavigationPage-specific behaviors
        if (page is NavigationPage navPage)
        {
            new NavigationPageActiveAwareBehavior().Apply(navPage);
            new PageSystemBackBehavior().Apply(navPage);
        }

        // TabbedPage-specific behaviors
        if (page is TabbedPage tabbedPage)
        {
            new TabbedPageActiveAwareBehavior().Apply(tabbedPage);
        }

        // Phase 3: region cleanup
        new RegionCleanupBehavior().Apply(page);
    }
}
