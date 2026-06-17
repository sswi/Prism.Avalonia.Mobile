using Prism.Dialogs;

namespace SampleApp.Views;

public partial class DialogDemoView : Avalonia.Controls.UserControl, IDialogAware
{
    public DialogDemoView() => InitializeComponent();

    public DialogCloseListener RequestClose { get; set; }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
    public void OnDialogOpened(IDialogParameters parameters) { }
}
