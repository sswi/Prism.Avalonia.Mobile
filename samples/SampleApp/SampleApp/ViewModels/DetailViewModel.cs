using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using System.Collections.Generic;

namespace SampleApp.ViewModels;

public class DetailViewModel : BindableBase, INavigatedAware, IConfirmNavigation
{
    private readonly INavigationService _nav;
    private string _paramDisplay = string.Empty;
    private string _lifecycleInfo = string.Empty;

    public string ParamDisplay { get => _paramDisplay; set => SetProperty(ref _paramDisplay, value); }
    public string LifecycleInfo { get => _lifecycleInfo; set => SetProperty(ref _lifecycleInfo, value); }

    public DelegateCommand NavigateDeeperCommand { get; }
    public DelegateCommand GoBackCommand { get; }
    public DelegateCommand GoBackToRootCommand { get; }
    public DelegateCommand NavigateWithConfirmationCommand { get; }

    public DetailViewModel(INavigationService nav)
    {
        _nav = nav;
        NavigateDeeperCommand = new(async () => await _nav.NavigateAsync("SubDetailPage?from=detail&depth=3"));
        GoBackCommand = new(async () => await _nav.GoBackAsync());
        GoBackToRootCommand = new(async () => await _nav.GoBackToRootAsync());
        NavigateWithConfirmationCommand = new(async () => await _nav.NavigateAsync("SubDetailPage?confirmed=true"));
    }

    public void OnNavigatedFrom(INavigationParameters p) => LifecycleInfo = "<- Navigated FROM";
    public void OnNavigatedTo(INavigationParameters p)
    {
        LifecycleInfo = "-> Navigated TO";
        var parts = new List<string>();
        if (p.TryGetValue<string>("source", out var s)) parts.Add($"Source: {s}");
        if (p.TryGetValue<string>("id", out var id)) parts.Add($"ID: {id}");
        if (p.TryGetValue<string>("from", out var f)) parts.Add($"From: {f}");
        ParamDisplay = string.Join(" | ", parts);
    }

    public bool CanNavigate(INavigationParameters p) => true;
}
