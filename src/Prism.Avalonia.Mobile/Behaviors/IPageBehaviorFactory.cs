using Avalonia.Controls;

namespace Prism.Behaviors;

/// <summary>
/// Factory that applies page-level behaviors to every page created
/// by the navigation system. Registered as a singleton; implementations
/// are resolved during page configuration.
/// </summary>
public interface IPageBehaviorFactory
{
    /// <summary>
    /// Applies all configured page behaviors to the given page.
    /// Called once per page when it is created by the navigation registry.
    /// </summary>
    /// <param name="page">The page to apply behaviors to.</param>
    void ApplyPageBehaviors(Page page);
}
