#nullable enable
using Avalonia;
using Avalonia.Controls;

namespace Prism.Common;

/// <summary>
/// Extension methods for Avalonia objects.
/// </summary>
internal static class AvaloniaObjectExtensions
{
    /// <summary>
    /// Attempts to auto-wire the ViewModel for the given view if it
    /// doesn't already have a DataContext set.
    /// </summary>
    public static void AutowireViewModel(AvaloniaObject view)
    {
        if (view is StyledElement styled && styled.DataContext is not null)
            return;

        Mvvm.ViewModelLocator.Autowire(view);
    }
}
