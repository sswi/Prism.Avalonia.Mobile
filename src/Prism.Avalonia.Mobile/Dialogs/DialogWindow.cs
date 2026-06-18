using Avalonia.Controls;

namespace Prism.Dialogs;

public class DialogWindow : Window, IDialogWindow
{
    public IDialogResult? Result { get; set; }

    public new Task ShowDialog(object owner)
    {
        if (owner is Window w && w != this)
            return base.ShowDialog<object>(w);
        return base.ShowDialog<object>(null!);
    }

    public new void Show() => base.Show();

    public new void Close()
    {
        Result ??= new DialogResult { Result = ButtonResult.None };
        base.Close();
    }
}
