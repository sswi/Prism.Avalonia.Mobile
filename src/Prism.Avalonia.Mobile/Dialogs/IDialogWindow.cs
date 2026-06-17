namespace Prism.Dialogs;

public interface IDialogWindow
{
    IDialogResult? Result { get; set; }
    object? Content { get; set; }
    object? DataContext { get; set; }

    event EventHandler? Closed;

    void Show();
    Task ShowDialog(object owner);
    void Close();
}
