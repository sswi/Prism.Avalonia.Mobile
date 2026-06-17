#nullable enable
using Avalonia;

namespace Prism.Navigation.Regions.Adapters;

/// <summary>
/// Maps Avalonia control types to <see cref="IRegionAdapter"/> types.
/// When a control with a <c>RegionManager.RegionName</c> is encountered,
/// the matching adapter is used to create a region for that control.
/// </summary>
public class RegionAdapterMappings
{
    private readonly Dictionary<Type, Type> _mappings = new();

    /// <summary>
    /// Registers a mapping from a control type to a region adapter type.
    /// </summary>
    /// <typeparam name="TControl">The control type.</typeparam>
    /// <typeparam name="TAdapter">The adapter type.</typeparam>
    public void RegisterMapping<TControl, TAdapter>()
        where TControl : AvaloniaObject
        where TAdapter : IRegionAdapter
    {
        _mappings[typeof(TControl)] = typeof(TAdapter);
    }

    /// <summary>
    /// Gets the adapter type for a given control type.
    /// </summary>
    /// <param name="controlType">The control type.</param>
    /// <returns>The adapter type, or null if no mapping exists.</returns>
    public Type? GetAdapterType(Type controlType)
    {
        // Check exact match first, then walk up the inheritance chain
        var currentType = controlType;
        while (currentType is not null && currentType != typeof(object))
        {
            if (_mappings.TryGetValue(currentType, out var adapterType))
                return adapterType;
            currentType = currentType.BaseType;
        }
        return null;
    }

    /// <summary>
    /// Gets the registered mappings.
    /// </summary>
    public IEnumerable<KeyValuePair<Type, Type>> GetMappings() => _mappings;
}
