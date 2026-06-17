using DryIoc;
using Prism.Ioc;

namespace Prism.DryIoc;

/// <summary>
/// DryIoc-specific base application class for Prism.Avalonia.Mobile.
/// Provides the DryIoc container integration.
/// </summary>
/// <remarks>
/// <para>Usage:</para>
/// <code>
/// public partial class App : Prism.DryIoc.PrismApplication
/// {
///     protected override AvaloniaObject CreateShell()
///         => Container.Resolve&lt;NavigationPage&gt;();
///
///     protected override void RegisterTypes(IContainerRegistry cr)
///     {
///         cr.RegisterForNavigation&lt;MainPage, MainViewModel&gt;("MainPage");
///     }
/// }
/// </code>
/// </remarks>
public abstract class PrismApplication : Prism.PrismApplication
{
    /// <summary>
    /// Creates the default container rules. Override to customize DryIoc behavior.
    /// For AOT scenarios, consider disabling dynamic registrations.
    /// </summary>
    protected virtual Rules CreateContainerRules()
    {
        return DryIocContainerExtension.DefaultRules;
    }

    /// <inheritdoc />
    protected override IContainerExtension CreateContainerExtension()
    {
        return new DryIocContainerExtension(CreateContainerRules());
    }

    /// <inheritdoc />
    protected override void RegisterFrameworkExceptionTypes()
    {
        ExceptionExtensions.RegisterFrameworkExceptionType(typeof(ContainerException));
    }
}
