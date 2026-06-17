namespace Prism.Navigation.Builder;

/// <summary>
/// Extension methods to create <see cref="INavigationBuilder"/> instances
/// from <see cref="INavigationService"/>.
/// </summary>
public static class NavigationBuilderExtensions
{
    /// <summary>
    /// Creates a new navigation builder to construct a URI and navigate.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    /// <returns>A new <see cref="INavigationBuilder"/>.</returns>
    /// <example>
    /// <code>
    /// var result = await _navigationService.CreateBuilder()
    ///     .AddSegment("DetailPage")
    ///     .AddParameter("id", 5)
    ///     .NavigateAsync();
    /// </code>
    /// </example>
    public static INavigationBuilder CreateBuilder(this INavigationService navigationService)
    {
        return new NavigationBuilder(navigationService);
    }

    /// <summary>
    /// Navigates using a builder action.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="configure">The builder configuration.</param>
    /// <returns>The navigation result.</returns>
    /// <example>
    /// <code>
    /// var result = await _navigationService.NavigateAsync(b => b
    ///     .AddSegment("DetailPage")
    ///     .AddParameter("id", 5));
    /// </code>
    /// </example>
    public static Task<INavigationResult> NavigateAsync(
        this INavigationService navigationService,
        Action<INavigationBuilder> configure)
    {
        var builder = new NavigationBuilder(navigationService);
        configure(builder);
        return builder.NavigateAsync();
    }
}
