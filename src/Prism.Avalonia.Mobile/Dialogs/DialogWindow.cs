using Avalonia.Controls;

namespace Prism.Dialogs;

/// <summary>
/// Default implementation of <see cref="IDialogWindow"/> using an Avalonia
/// <see cref="Window"/>.
/// </summary>
public class DialogWindow : Window, IDialogWindow
{
    /// <inheritdoc />
    public IDialogResult? Result { get; set; }

    /// <inheritdoc />
    public new Task ShowDialog(object owner)
    {
        if (owner is Window w && w != this)
            return base.ShowDialog<object>(w);
        return base.ShowDialog<object>(null!);
    }

    /// <inheritdoc />
    public new void Show() => base.Show();

    /// <inheritdoc />
    public new void Close()
    {
        if (Result is null)
            Result = new DialogResult { Result = ButtonResult.None };
        base.Close();
    }
}
