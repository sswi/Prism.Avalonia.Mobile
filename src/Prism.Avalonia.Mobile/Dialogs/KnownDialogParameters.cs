namespace Prism.Dialogs;

/// <summary>
/// Well-known dialog parameter keys.
/// </summary>
public static class KnownDialogParameters
{
    /// <summary>
    /// When <c>true</c>, the dialog is presented as an inline modal page.
    /// When <c>false</c>, opens as a separate OS window. Default is <c>false</c>.
    /// </summary>
    public const string UseInlineModal = "useInlineModal";

    /// <summary>
    /// When <c>true</c>, tapping the backdrop closes the dialog.
    /// Default is <c>true</c>. Only applies to inline modal dialogs.
    /// </summary>
    public const string CloseOnBackdropTap = "closeOnBackdropTap";

    /// <summary>
    /// Contains the <see cref="IDialogCloser"/> instance injected by the
    /// dialog service. The dialog ViewModel can extract and call
    /// <c>Closer.Close(result)</c> to dismiss with a result.
    /// </summary>
    public const string DialogCloser = "__DialogCloser";
}
