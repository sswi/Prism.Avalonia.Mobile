namespace Prism.Navigation.Builder;

/// <summary>
/// Fluent builder for constructing navigation URIs programmatically.
/// Aligned with Prism MAUI's <c>INavigationBuilder</c>.
/// </summary>
/// <example>
/// <code>
/// var result = await _navigationService.CreateBuilder()
///     .AddSegment("DetailPage", s => s.UseModalNavigation())
///     .AddParameter("id", 5)
///     .NavigateAsync();
/// </code>
/// </example>
public interface INavigationBuilder
{
    /// <summary>
    /// Adds a page segment to the navigation URI.
    /// </summary>
    /// <param name="segmentName">The navigation name of the page.</param>
    /// <param name="configureSegment">Optional configuration for the segment.</param>
    INavigationBuilder AddSegment(string segmentName, Action<ISegmentBuilder>? configureSegment = null);

    /// <summary>
    /// Adds a tabbed page segment to the navigation URI.
    /// </summary>
    /// <param name="configureTabbedSegment">Configuration for the tabbed segment.</param>
    INavigationBuilder AddTabbedSegment(Action<ITabbedSegmentBuilder> configureTabbedSegment);

    /// <summary>
    /// Adds a parameter that applies to the entire navigation.
    /// </summary>
    INavigationBuilder AddParameter(string key, object? value);

    /// <summary>
    /// Sets whether navigation should use modal presentation.
    /// </summary>
    INavigationBuilder UseModalNavigation(bool useModal = true);

    /// <summary>
    /// Sets whether navigation should be animated.
    /// </summary>
    INavigationBuilder UseAnimatedNavigation(bool animated = true);

    /// <summary>
    /// Gets the constructed navigation URI.
    /// </summary>
    Uri GetNavigationUri();

    /// <summary>
    /// Gets the accumulated navigation parameters.
    /// </summary>
    INavigationParameters GetNavigationParameters();

    /// <summary>
    /// Executes the navigation asynchronously.
    /// </summary>
    Task<INavigationResult> NavigateAsync();
}
