using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;

namespace SampleApp.ViewModels;

public class DialogDemoViewModel : BindableBase, IDialogAware
{
    public DialogCloseListener RequestClose { get; set; }
    public bool CanCloseDialog() => true;
    public void OnDialogClosed() { }
    private IDialogCloser? _closer;
    private string _title = string.Empty;
    private string _message = string.Empty;
    private string _userInput = string.Empty;

    public string Title { get => _title; set => SetProperty(ref _title, value); }
    public string Message { get => _message; set => SetProperty(ref _message, value); }
    public string UserInput { get => _userInput; set => SetProperty(ref _userInput, value); }

    public DelegateCommand CloseCommand { get; }
    public DelegateCommand CancelCommand { get; }

    public DialogDemoViewModel()
    {
        CloseCommand = new(() => _closer?.Close(new DialogResult
        {
            Result = ButtonResult.OK,
            Parameters = new DialogParameters { { "userInput", UserInput } }
        }));
        CancelCommand = new(() => _closer?.Close(new DialogResult
        {
            Result = ButtonResult.Cancel,
            Parameters = new DialogParameters { { "reason", "cancelled" } }
        }));
    }

    public void OnDialogOpened(IDialogParameters p)
    {
        if (p.TryGetValue<IDialogCloser>(KnownDialogParameters.DialogCloser, out var c)) _closer = c;
        if (p.TryGetValue<string>("title", out var t)) Title = t;
        if (p.TryGetValue<string>("message", out var m)) Message = m;
    }
}
