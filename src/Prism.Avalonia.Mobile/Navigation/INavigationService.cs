namespace Prism.Navigation;

/// <summary>
/// Provides page-based navigation for ViewModels — fully aligned with the Prism MAUI
/// <c>INavigationService</c> contract. In Avalonia 12, this wraps <see cref="Avalonia.Controls.NavigationPage"/>
/// and its <c>INavigation</c> interface.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Navigates to the most recent entry in the back navigation history by popping
    /// the calling Page off the navigation stack.
    /// </summary>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>
    /// An <see cref="INavigationResult"/> indicating whether the request was successful
    /// or if there was an encountered <see cref="Exception"/>.
    /// </returns>
    Task<INavigationResult> GoBackAsync(INavigationParameters? parameters = null);

    /// <summary>
    /// Navigates to the most recent entry in the back navigation history for the
    /// specified <paramref name="viewName"/>.
    /// </summary>
    /// <param name="viewName">The name of the View to navigate back to.</param>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>
    /// An <see cref="INavigationResult"/> indicating whether the request was successful
    /// or if there was an encountered <see cref="Exception"/>.
    /// </returns>
    Task<INavigationResult> GoBackToAsync(string viewName, INavigationParameters? parameters = null);

    /// <summary>
    /// When navigating inside a NavigationPage: pops all but the root Page off the
    /// navigation stack.
    /// </summary>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>
    /// An <see cref="INavigationResult"/> indicating whether the request was successful
    /// or if there was an encountered <see cref="Exception"/>.
    /// </returns>
    /// <remarks>Only works when called from a View within a NavigationPage.</remarks>
    Task<INavigationResult> GoBackToRootAsync(INavigationParameters? parameters = null);

    /// <summary>
    /// Initiates navigation to the target specified by the <paramref name="uri"/>.
    /// </summary>
    /// <param name="uri">The Uri to navigate to.</param>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>
    /// An <see cref="INavigationResult"/> indicating whether the request was successful
    /// or if there was an encountered <see cref="Exception"/>.
    /// </returns>
    /// <remarks>
    /// Navigation parameters can be provided in the Uri and by using the
    /// <paramref name="parameters"/> argument.
    /// </remarks>
    /// <example>
    /// NavigateAsync(new Uri("MainPage?id=3&amp;name=Brian", UriKind.Relative), parameters);
    /// </example>
    Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters? parameters = null);

    /// <summary>
    /// Selects a tab of the TabbedPage parent and optionally navigates to a specified Uri
    /// within that tab.
    /// </summary>
    /// <param name="name">The name of the tab to select.</param>
    /// <param name="uri">
    /// The optional Uri to navigate to within the selected tab. If <c>null</c>, only the
    /// tab is selected without further navigation.
    /// </param>
    /// <param name="parameters">The navigation parameters.</param>
    /// <returns>
    /// An <see cref="INavigationResult"/> indicating whether the request was successful
    /// or if there was an encountered <see cref="Exception"/>.
    /// </returns>
    Task<INavigationResult> SelectTabAsync(string name, Uri? uri = null, INavigationParameters? parameters = null);
}
