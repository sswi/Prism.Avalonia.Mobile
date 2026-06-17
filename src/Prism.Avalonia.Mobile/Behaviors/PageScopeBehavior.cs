using Avalonia;
using Avalonia.Controls;
using Prism.Ioc;

namespace Prism.Behaviors;

internal sealed class PageScopeBehavior
{
    private static readonly AttachedProperty<IContainerProvider?> ScopeProperty =
        AvaloniaProperty.RegisterAttached<PageScopeBehavior, Page, IContainerProvider?>("ScopeContainer");

    public void Apply(Page page, IContainerProvider scope)
    {
        page.SetValue(ScopeProperty, scope);
    }
}
