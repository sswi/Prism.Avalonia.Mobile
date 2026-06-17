namespace Prism.Navigation;

/// <summary>
/// When implemented by a page, controls whether navigating to that page
/// inside a <see cref="Avalonia.Controls.NavigationPage"/> should first
/// clear the navigation stack.
/// </summary>
public interface INavigationPageOptions
{
    /// <summary>
    /// Gets a value indicating whether the navigation stack should be cleared
    /// before pushing this page.
    /// </summary>
    bool ClearNavigationStackOnNavigation { get; }
}
