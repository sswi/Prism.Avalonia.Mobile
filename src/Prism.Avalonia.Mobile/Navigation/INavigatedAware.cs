namespace Prism.Navigation;

/// <summary>
/// Called after the page has been navigated to.
/// </summary>
public interface INavigatedAware
{
    /// <summary>
    /// Called when the implementer has been navigated away from.
    /// </summary>
    void OnNavigatedFrom(INavigationParameters parameters);

    /// <summary>
    /// Called when the implementer has been navigated to.
    /// </summary>
    void OnNavigatedTo(INavigationParameters parameters);
}
