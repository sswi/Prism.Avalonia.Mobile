namespace Prism.Dialogs;

/// <summary>
/// Provides an AOT-safe close mechanism for dialogs.
/// The dialog service injects an implementation via IDialogParameters
/// before calling OnDialogOpened. The dialog View/ViewModel calls
/// <see cref="Close"/> to dismiss the dialog with a result.
/// </summary>
public interface IDialogCloser
{
    /// <summary>
    /// Closes the dialog with the given result. The result carries
    /// ButtonResult (OK/Cancel/None) and optional return parameters.
    /// </summary>
    void Close(IDialogResult result);
}
