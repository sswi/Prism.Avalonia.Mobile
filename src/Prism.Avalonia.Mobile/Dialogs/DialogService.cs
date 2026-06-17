#nullable enable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Dialogs;

public class DialogService : IDialogService
{
    private readonly IContainerExtension _container;

    public DialogService(IContainerExtension container)
        => _container = container ?? throw new ArgumentNullException(nameof(container));

    public void Show(string name, IDialogParameters parameters, DialogCallback callback)
        => ShowInternal(name, parameters, callback, modal: false, closeOnBackdropTap: true);

    public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        => ShowInternal(name, parameters, callback, modal: true, closeOnBackdropTap: true);

    private void ShowInternal(string name, IDialogParameters parameters, DialogCallback callback, bool modal, bool closeOnBackdropTap)
    {
        var useInline = DialogServiceExtensions.IsMobilePlatform;
        if (!useInline && parameters.TryGetValue<bool>(KnownDialogParameters.UseInlineModal, out var c)) useInline = c;
        if (parameters.TryGetValue<bool>(KnownDialogParameters.CloseOnBackdropTap, out var cb)) closeOnBackdropTap = cb;

        if (useInline)
            _ = ShowInlineAsync(name, parameters, callback, closeOnBackdropTap);
        else
            ShowWindow(name, parameters, callback, modal);
    }

    private void ShowWindow(string name, IDialogParameters parameters, DialogCallback callback, bool modal)
    {
        var window = _container.Resolve<IDialogWindow>();
        var content = CreateContent(name, parameters);
        if (content is null) return;
        window.Content = content;

        void OnClosed(object? s, EventArgs e)
        {
            window.Closed -= OnClosed;
            if (content is IDialogAware da) da.OnDialogClosed();
            callback.Invoke(window.Result ?? new DialogResult { Result = ButtonResult.None });
        }
        window.Closed += OnClosed;
        if (modal) window.ShowDialog(GetOwnerWindow()!);
        else window.Show();
    }

    private async Task ShowInlineAsync(string name, IDialogParameters parameters, DialogCallback callback, bool closeOnBackdropTap)
    {
        var navPage = FindNavPage();
        if (navPage is null) { ShowWindow(name, parameters, callback, modal: true); return; }

        var content = CreateContent(name, parameters);
        if (content is not Control ctrl) return;

        var contentAware = FindAware(ctrl);

        // Build close action: fires OnDialogClosed + callback + pops modal
        Action<IDialogResult> closeAction = result =>
        {
            contentAware?.OnDialogClosed();
            callback.Invoke(result);

            if (navPage is INavigation n)
                _ = n.PopModalAsync();
        };

        var dialogPage = new DialogContainerPage(ctrl, closeAction, closeOnBackdropTap);

        // Inject IDialogCloser so dialog ViewModel can close itself (AOT-safe!)
        parameters.Add(KnownDialogParameters.DialogCloser, (IDialogCloser)dialogPage);

        if (contentAware is not null)
        {
            contentAware.OnDialogOpened(parameters);

            // Also pass to ViewModel if it implements IDialogAware
            if (content is StyledElement se && se.DataContext is IDialogAware vmAware)
            {
                vmAware.OnDialogOpened(parameters);
            }
        }

        if (navPage is INavigation nav)
            await nav.PushModalAsync(dialogPage);
    }

    private object? CreateContent(string name, IDialogParameters parameters)
    {
        var content = _container.Resolve<object>(name);
        if (content is AvaloniaObject ao) ViewModelLocator.Autowire(ao);
        return content;
    }

    private static IDialogAware? FindAware(object? c)
    {
        if (c is IDialogAware da) return da;
        if (c is ContentControl cc) return FindAware(cc.Content);
        return null;
    }

    private static NavigationPage? FindNavPage()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d && d.MainWindow is { } w)
            return FindNav(w.Content);
        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime s && s.MainView is { } v)
            return FindNav(v);
        return null;
    }

    private static NavigationPage? FindNav(object? r)
    {
        if (r is NavigationPage np) return np;
        if (r is ContentControl cc && cc.Content is not null) return FindNav(cc.Content);
        if (r is Visual v) foreach (var c in Avalonia.VisualTree.VisualExtensions.GetVisualChildren(v)) { var x = FindNav(c); if (x is not null) return x; }
        return null;
    }

    private static Window? GetOwnerWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime d) return d.MainWindow;
        return null;
    }
}
