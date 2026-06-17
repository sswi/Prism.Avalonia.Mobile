namespace Prism.Navigation;

/// <summary>
/// Convenience extension methods for <see cref="INavigationService"/>.
/// </summary>
public static class INavigationServiceExtensions
{
    /// <summary>
    /// Navigates to the specified URI.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="uri">The URI to navigate to (relative or absolute).</param>
    /// <param name="parameters">Optional navigation parameters.</param>
    /// <param name="useModalNavigation">If <c>true</c>, uses modal navigation.</param>
    /// <param name="animated">If <c>false</c>, suppresses animations.</param>
    public static Task<INavigationResult> NavigateAsync(
        this INavigationService navigationService,
        string uri,
        INavigationParameters? parameters = null,
        bool? useModalNavigation = null,
        bool animated = true)
    {
        parameters ??= new NavigationParameters();

        if (useModalNavigation.HasValue)
            parameters.Add(KnownNavigationParameters.UseModalNavigation, useModalNavigation.Value);

        if (!animated)
            parameters.Add(KnownNavigationParameters.Animated, false);

        return navigationService.NavigateAsync(
            new Uri(uri, UriKind.RelativeOrAbsolute),
            parameters);
    }

    /// <summary>
    /// Navigates to the specified view by name.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="name">The registered navigation name of the view.</param>
    /// <param name="parameters">Optional navigation parameters.</param>
    public static Task<INavigationResult> NavigateAsync(
        this INavigationService navigationService,
        string name,
        INavigationParameters parameters)
    {
        return navigationService.NavigateAsync(
            new Uri(name, UriKind.Relative),
            parameters);
    }

    /// <summary>
    /// Selects a tab by name with optional navigation within the tab.
    /// </summary>
    /// <param name="navigationService">The navigation service.</param>
    /// <param name="name">The name of the tab to select.</param>
    /// <param name="uri">Optional URI to navigate to within the selected tab.</param>
    /// <param name="parameters">Optional navigation parameters.</param>
    public static Task<INavigationResult> SelectTabAsync(
        this INavigationService navigationService,
        string name,
        string? uri = null,
        INavigationParameters? parameters = null)
    {
        return navigationService.SelectTabAsync(
            name,
            uri is not null ? new Uri(uri, UriKind.Relative) : null,
            parameters);
    }

    /// <summary>
    /// Navigates back to the root of the current NavigationPage.
    /// </summary>
    public static Task<INavigationResult> GoBackToRootAsync(
        this INavigationService navigationService)
    {
        return navigationService.GoBackToRootAsync(null);
    }
}
