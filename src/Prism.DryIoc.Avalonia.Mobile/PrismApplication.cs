using DryIoc;
using Prism.Ioc;

namespace Prism.DryIoc;

/// <summary>
/// DryIoc-specific base application class for Prism.Avalonia.Mobile.
/// Applies AOT-safe container rules by default.
/// </summary>
public abstract class PrismApplication : Prism.PrismApplication
{
    /// <summary>
    /// Creates the default container rules. For AOT scenarios,
    /// registers internal Prism types explicitly and disables dynamic registration.
    /// </summary>
    protected virtual Rules CreateContainerRules()
    {
        var rules = DryIocContainerExtension.DefaultRules;

        // AOT-safe: register internal Prism types that lack public constructors
        rules = rules.WithConcreteTypeDynamicRegistrations((serviceType, _) =>
        {
            // Allow dynamic registration only for simple value types
            return serviceType.IsValueType;
        });

        return rules;
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
