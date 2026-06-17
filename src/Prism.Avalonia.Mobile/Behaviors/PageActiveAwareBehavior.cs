using Avalonia.Controls;
using Prism.Common;

namespace Prism.Behaviors;

/// <summary>
/// Tracks whether a page is the currently active page within its
/// <see cref="NavigationPage"/> or <see cref="TabbedPage"/>,
/// and dispatches <see cref="IActiveAware"/> notifications.
/// </summary>
internal sealed class PageActiveAwareBehavior
{
    private Page? _page;
    private bool _isActive;

    public void Apply(Page page)
    {
        _page = page;

        // Track when the page is attached/detached
        page.AttachedToVisualTree += OnAttached;
        page.DetachedFromVisualTree += OnDetached;
    }

    private void OnAttached(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        SetActive(true);
    }

    private void OnDetached(object? sender, Avalonia.VisualTreeAttachmentEventArgs e)
    {
        SetActive(false);
    }

    private void SetActive(bool isActive)
    {
        if (_isActive == isActive) return;
        _isActive = isActive;

        if (_page is not null)
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(
                _page, a => a.IsActive = isActive);
        }
    }
}
