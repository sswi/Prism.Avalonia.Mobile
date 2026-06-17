namespace Prism.Navigation;

/// <summary>
/// Well-known navigation parameter keys used internally by Prism.
/// </summary>
public static class KnownNavigationParameters
{
    /// <summary>
    /// When <c>true</c>, creates a new tab when navigating within a TabbedPage.
    /// </summary>
    public const string CreateTab = "createTab";

    /// <summary>
    /// The name of the tab to select within a TabbedPage.
    /// </summary>
    public const string SelectedTab = "selectedTab";

    /// <summary>
    /// The title to use when creating a new tab.
    /// </summary>
    public const string Title = "title";

    /// <summary>
    /// When <c>true</c>, uses modal navigation (PushModalAsync) instead of stack
    /// navigation (PushAsync).
    /// </summary>
    public const string UseModalNavigation = "useModalNavigation";

    /// <summary>
    /// When <c>true</c>, suppresses the default page transition animation.
    /// </summary>
    public const string Animated = "animated";

    /// <summary>
    /// When <c>true</c>, clears the navigation stack before pushing the new page.
    /// Only applies when navigating within a NavigationPage.
    /// </summary>
    public const string ClearNavigationStack = "clearNavigationStack";

    /// <summary>
    /// An XAML-sourced parameter value.
    /// </summary>
    public const string XamlParam = "xamlParam";
}
