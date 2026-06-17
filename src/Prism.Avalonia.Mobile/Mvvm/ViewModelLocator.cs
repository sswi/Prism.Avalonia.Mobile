#nullable enable
using Avalonia;

namespace Prism.Mvvm;

/// <summary>
/// Avalonia implementation of ViewModel auto-wiring via the
/// <c>AutoWireViewModel</c> attached property.
/// </summary>
public static class ViewModelLocator
{
    /// <summary>
    /// Identifies the AutoWireViewModel attached property.
    /// </summary>
    public static readonly AttachedProperty<bool> AutoWireViewModelProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaObject, bool>(
            "AutoWireViewModel",
            typeof(ViewModelLocator));

    /// <summary>
    /// Gets the value of AutoWireViewModel.
    /// </summary>
    public static bool GetAutoWireViewModel(AvaloniaObject obj) =>
        obj.GetValue(AutoWireViewModelProperty);

    /// <summary>
    /// Sets the value of AutoWireViewModel.
    /// </summary>
    public static void SetAutoWireViewModel(AvaloniaObject obj, bool value) =>
        obj.SetValue(AutoWireViewModelProperty, value);

    /// <summary>
    /// Explicitly triggers ViewModel auto-wiring. Called by the navigation system.
    /// </summary>
    public static void Autowire(AvaloniaObject view)
    {
        // Skip if already has DataContext
        if (view is StyledElement styled && styled.DataContext is not null)
            return;

        // Directly resolve ViewModel via ViewModelLocationProvider
        ViewModelLocationProvider.AutoWireViewModelChanged(view, (v, vm) =>
        {
            if (v is StyledElement s)
                s.DataContext = vm;
        });
    }
}
