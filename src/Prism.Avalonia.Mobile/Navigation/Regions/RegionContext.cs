using Avalonia;

namespace Prism.Navigation.Regions;

/// <summary>
/// Observable context shared between a region host control and its contained views.
/// </summary>
public class RegionContext : AvaloniaObject
{
    /// <summary>
    /// Identifies the Value attached property.
    /// </summary>
    public static readonly AttachedProperty<object?> RegionContextValueProperty =
        AvaloniaProperty.RegisterAttached<RegionContext, AvaloniaObject, object?>(
            "RegionContextValue");

    /// <summary>
    /// Gets the RegionContext value from a control.
    /// </summary>
    public static object? GetRegionContextValue(AvaloniaObject obj) =>
        obj.GetValue(RegionContextValueProperty);

    /// <summary>
    /// Sets the RegionContext value on a control.
    /// </summary>
    public static void SetRegionContextValue(AvaloniaObject obj, object? value) =>
        obj.SetValue(RegionContextValueProperty, value);
}
