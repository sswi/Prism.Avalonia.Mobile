#nullable enable
using Avalonia.Controls;
using Prism.Behaviors;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Navigation;

internal sealed class NavigationRegistry : AvaloniaViewRegistryBase, INavigationRegistry
{
    private readonly IPageBehaviorFactory _behaviorFactory;

    public NavigationRegistry(IPageBehaviorFactory behaviorFactory)
        : base(ViewType.Page, Prism.Ioc.AvaloniaContainerRegistryExtensions.PendingRegistrations.Where(r => r.Type == ViewType.Page))
    {
        _behaviorFactory = behaviorFactory ?? throw new ArgumentNullException(nameof(behaviorFactory));
    }

    protected override void ConfigureView(Avalonia.AvaloniaObject? view, IContainerProvider container)
    {
        if (view is not Page page) return;
        ConfigurePage(container, page);
    }

    private void ConfigurePage(IContainerProvider container, Page page)
    {
        // TabbedPage children are configured individually when they are navigated to
        if (page is NavigationPage navPage && navPage.Content is Page rootPage)
        {
            var s = container.CreateScope();
            ConfigurePage(s, rootPage);
        }

        // Attach container provider
        var cp = AvaloniaViewRegistryBase.GetContainerProvider(page);
        if (cp is null)
        {
            var prop = Avalonia.AvaloniaProperty.RegisterAttached<NavigationRegistry, Page, IContainerProvider?>("__ScopedContainer");
            page.SetValue(prop, container);
        }

        var accessor = container.Resolve<IPageAccessor>();
        accessor.Page ??= page;

        _behaviorFactory.ApplyPageBehaviors(page);
    }
}
