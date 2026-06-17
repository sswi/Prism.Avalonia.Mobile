using Avalonia.Controls;
using Prism.Common;

namespace Prism.Behaviors;

/// <summary>
/// Hooks into Avalonia page lifecycle events and dispatches them to the
/// page and its ViewModel via the <c>IPageLifecycleAware</c> interface.
/// </summary>
/// <remarks>
/// <para>In Avalonia, page lifecycle is tracked via:</para>
/// <list type="bullet">
///   <item><c>AttachedToVisualTree</c> — page appeared</item>
///   <item><c>DetachedFromVisualTree</c> — page disappeared</item>
/// </list>
/// </remarks>
internal sealed class PageLifeCycleAwareBehavior
{
    public void Apply(Page page)
    {
        page.AttachedToVisualTree += (s, e) =>
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IPageLifecycleAware>(
                page, a => a.OnAppearing());
        };

        page.DetachedFromVisualTree += (s, e) =>
        {
            MvvmHelpers.InvokeViewAndViewModelAction<IPageLifecycleAware>(
                page, a => a.OnDisappearing());
        };
    }
}

/// <summary>
/// Interface for page lifecycle notifications. Implement on View or ViewModel
/// to receive appearing/disappearing events.
/// </summary>
public interface IPageLifecycleAware
{
    /// <summary>
    /// Called when the page appears (is attached to the visual tree).
    /// </summary>
    void OnAppearing();

    /// <summary>
    /// Called when the page disappears (is detached from the visual tree).
    /// </summary>
    void OnDisappearing();
}
