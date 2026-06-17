namespace Prism.Dialogs;

/// <summary>
/// Extension methods for <see cref="IDialogService"/> that allow
/// choosing between inline modal (mobile-friendly, via NavigationPage)
/// and external window (desktop, OS-level popup).
/// </summary>
public static class DialogServiceExtensions
{
    /// <summary>
    /// Shows a modal dialog as an inline modal page on the NavigationPage stack.
    /// Always inline — ignores platform detection.
    /// </summary>
    public static void ShowInline(
        this IDialogService dialogService,
        string name,
        IDialogParameters parameters,
        DialogCallback callback)
    {
        parameters.Add(KnownDialogParameters.UseInlineModal, true);
        dialogService.ShowDialog(name, parameters, callback);
    }

    /// <summary>
    /// Shows a modal dialog as an external OS window.
    /// Falls back to inline modal on mobile/browser platforms.
    /// </summary>
    public static void ShowWindow(
        this IDialogService dialogService,
        string name,
        IDialogParameters parameters,
        DialogCallback callback)
    {
        parameters.Add(KnownDialogParameters.UseInlineModal, false);
        dialogService.ShowDialog(name, parameters, callback);
    }

    /// <summary>
    /// Detects whether the current platform requires inline modal dialogs.
    /// </summary>
    internal static bool IsMobilePlatform =>
        OperatingSystem.IsAndroid() ||
        OperatingSystem.IsIOS() ||
        OperatingSystem.IsBrowser() ||
        OperatingSystem.IsTvOS() ||
        OperatingSystem.IsWatchOS();
}
