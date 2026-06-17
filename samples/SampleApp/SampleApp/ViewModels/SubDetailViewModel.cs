using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace SampleApp.ViewModels;

public class SubDetailViewModel : BindableBase, INavigatedAware
{
    private readonly INavigationService _nav;
    private string _depthInfo = string.Empty;
    public string DepthInfo { get => _depthInfo; set => SetProperty(ref _depthInfo, value); }

    public DelegateCommand GoBackCommand { get; }
    public DelegateCommand GoBackToMainCommand { get; }
    public DelegateCommand GoBackToRootCommand { get; }

    public SubDetailViewModel(INavigationService nav)
    {
        _nav = nav;
        GoBackCommand = new(async () => await _nav.GoBackAsync());
        GoBackToMainCommand = new(async () => await _nav.GoBackToAsync("MainPage"));
        GoBackToRootCommand = new(async () => await _nav.GoBackToRootAsync());
    }

    public void OnNavigatedFrom(INavigationParameters p) { }
    public void OnNavigatedTo(INavigationParameters p)
    {
        p.TryGetValue<string>("depth", out var d);
        p.TryGetValue<string>("from", out var f);
        DepthInfo = $"Depth: {d ?? "3"} | From: {f ?? "unknown"}\nUse GoBackTo 'MainPage' to skip a level.";
    }
}
