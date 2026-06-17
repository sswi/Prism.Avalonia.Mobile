namespace Prism.Navigation;

/// <summary>
/// Tracks the source of the current navigation operation, used to distinguish
/// programmatic navigations from device-initiated back button presses.
/// </summary>
internal static class PageNavigationSource
{
    /// <summary>
    /// Navigation was initiated by <see cref="INavigationService"/>.
    /// </summary>
    internal const string NavigationService = "NavigationService";

    /// <summary>
    /// Navigation was initiated by a device back button or system gesture.
    /// </summary>
    internal const string Device = "Device";

    /// <summary>
    /// Navigation was initiated by the dialog service.
    /// </summary>
    internal const string DialogService = "DialogService";
}
