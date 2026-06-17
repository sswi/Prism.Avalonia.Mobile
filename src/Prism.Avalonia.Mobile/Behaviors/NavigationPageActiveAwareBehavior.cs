using Avalonia.Controls;
using Prism.Common;

namespace Prism.Behaviors;

/// <summary>
/// Listens to <see cref="NavigationPage"/> push/pop events and dispatches
/// <see cref="IActiveAware"/> notifications to pages as they become the
/// active (topmost) page or are pushed into the background.
/// </summary>
internal sealed class NavigationPageActiveAwareBehavior
{
    private NavigationPage? _navigationPage;

    public void Apply(NavigationPage navigationPage)
    {
        _navigationPage = navigationPage;

        navigationPage.Pushed += OnPushed;
        navigationPage.Popped += OnPopped;

        // Set the current top page as active
        if (navigationPage.NavigationStack.Count > 0)
        {
            var topPage = navigationPage.NavigationStack[^1];
            SetPageActive(topPage, true);
        }
    }

    private void OnPushed(object? sender, NavigationEventArgs e)
    {
        // Set the previous top page to inactive
        if (_navigationPage?.NavigationStack.Count > 1)
        {
            var previous = _navigationPage.NavigationStack[^2];
            SetPageActive(previous, false);
        }
        SetPageActive(e.Page, true);
    }

    private void OnPopped(object? sender, NavigationEventArgs e)
    {
        SetPageActive(e.Page, false);

        // Set the new top page to active
        if (_navigationPage?.NavigationStack.Count > 0)
        {
            var newTop = _navigationPage.NavigationStack[^1];
            SetPageActive(newTop, true);
        }
    }

    private static void SetPageActive(Page? page, bool isActive)
    {
        if (page is null) return;
        MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(
            page, a => a.IsActive = isActive);
    }
}
