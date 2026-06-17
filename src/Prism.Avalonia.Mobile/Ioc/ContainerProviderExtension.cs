#nullable enable
using Avalonia.Markup.Xaml;

namespace Prism.Ioc;

/// <summary>
/// XAML markup extension that resolves a service from the Prism container.
/// Usage: <c>{prism:ContainerProvider {x:Type vm:MainViewModel}}</c>
/// </summary>
public sealed class ContainerProviderExtension : MarkupExtension
{
    /// <summary>
    /// Gets or sets the type to resolve from the container.
    /// </summary>
    public Type? Type { get; set; }

    /// <summary>
    /// Gets or sets the optional name used when resolving from the container.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Creates a new instance of <see cref="ContainerProviderExtension"/>.
    /// </summary>
    public ContainerProviderExtension()
    {
    }

    /// <summary>
    /// Creates a new instance with the specified type.
    /// </summary>
    /// <param name="type">The type to resolve.</param>
    public ContainerProviderExtension(Type type)
    {
        Type = type;
    }

    /// <inheritdoc />
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (Type is null)
            return new InvalidOperationException("ContainerProviderExtension requires a Type.");

        var container = ContainerLocator.Container;

        if (!string.IsNullOrWhiteSpace(Name))
            return container.Resolve(Type, Name);

        return container.Resolve(Type);
    }
}
