using Prism.Commands;
using Prism.Dialogs;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Builder;
using System.Threading.Tasks;

namespace SampleApp.ViewModels;

public class MainViewModel : BindableBase, INavigatedAware
{
    private readonly INavigationService _nav;
    private readonly IDialogService _dialog;

    private string _dialogResult = string.Empty;
    public string DialogResult { get => _dialogResult; set => SetProperty(ref _dialogResult, value); }

    public DelegateCommand NavigateDetailCommand { get; }
    public DelegateCommand BuilderNavigateCommand { get; }
    public DelegateCommand DeepNavigateCommand { get; }
    public DelegateCommand ShowDialogCommand { get; }
    public DelegateCommand ShowWindowCommand { get; }
    public DelegateCommand OpenEditCommand { get; }
    public DelegateCommand GoBackCommand { get; }

    public MainViewModel(INavigationService nav, IDialogService dialog)
    {
        _nav = nav;
        _dialog = dialog;

        NavigateDetailCommand = new(async () => await _nav.NavigateAsync("DetailPage?source=card&id=42"));

        BuilderNavigateCommand = new(async () => await _nav.CreateBuilder()
            .AddSegment("DetailPage", s => s.AddParameter("source", "builder").AddParameter("id", 99).UseAnimatedNavigation())
            .NavigateAsync());

        DeepNavigateCommand = new(async () => await _nav.NavigateAsync("DetailPage/SubDetailPage?from=deepNav"));

        ShowDialogCommand = new(() =>
        {
            var p = new DialogParameters { { "title", "Welcome!" }, { "message", "Type below and click OK to return data." } };
            _dialog.ShowInline("DemoDialog", p, new DialogCallback().OnClose(r =>
            {
                if (r.Result == ButtonResult.OK && r.Parameters?.TryGetValue<string>("userInput", out var input) == true)
                    DialogResult = $"Got: '{input}' (OK)";
                else if (r.Parameters?.TryGetValue<string>("reason", out var reason) == true)
                    DialogResult = $"Got: '{reason}' (Cancel)";
            }));
        });

        ShowWindowCommand = new(() =>
        {
            var p = new DialogParameters { { "title", "External Window" }, { "message", "This is an OS window dialog." } };
            _dialog.ShowWindow("DemoDialog", p, new DialogCallback().OnClose(r =>
                DialogResult = $"Window closed: {r.Result}"));
        });

        GoBackCommand = new(async () =>
        {
            await _nav.NavigateAsync("DetailPage?source=gobackDemo");
            await Task.Delay(300);
            var r = await _nav.GoBackAsync();
            DialogResult = r.Success ? "GoBack succeeded!" : "GoBack failed";
        });

        OpenEditCommand = new(async () =>
        {
            await _nav.NavigateAsync("EditPage");
        });
    }

    public void OnNavigatedFrom(INavigationParameters p) { }
    public void OnNavigatedTo(INavigationParameters p) { }
}
