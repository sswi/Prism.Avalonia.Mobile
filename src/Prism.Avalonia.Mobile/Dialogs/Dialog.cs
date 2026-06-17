using Avalonia;

namespace Prism.Dialogs;

/// <summary>
/// Provides attached properties for configuring dialog windows.
/// </summary>
public static class Dialog
{
    /// <summary>
    /// Identifies the WindowStyle attached property.
    /// </summary>
    public static readonly AttachedProperty<string> WindowStyleProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, string>(
            "WindowStyle",
            typeof(Dialog),
            defaultValue: "Normal");

    /// <summary>
    /// Gets the window style.
    /// </summary>
    public static string GetWindowStyle(AvaloniaObject obj) =>
        obj.GetValue(WindowStyleProperty);

    /// <summary>
    /// Sets the window style.
    /// </summary>
    public static void SetWindowStyle(AvaloniaObject obj, string value) =>
        obj.SetValue(WindowStyleProperty, value);

    /// <summary>
    /// Identifies the WindowStartupLocation attached property.
    /// </summary>
    public static readonly AttachedProperty<string> WindowStartupLocationProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, string>(
            "WindowStartupLocation",
            typeof(Dialog),
            defaultValue: "CenterOwner");

    /// <summary>
    /// Gets the window startup location.
    /// </summary>
    public static string GetWindowStartupLocation(AvaloniaObject obj) =>
        obj.GetValue(WindowStartupLocationProperty);

    /// <summary>
    /// Sets the window startup location.
    /// </summary>
    public static void SetWindowStartupLocation(AvaloniaObject obj, string value) =>
        obj.SetValue(WindowStartupLocationProperty, value);
}
