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
        => _ = ShowInlineAsync(name, parameters, callback, closeOnBackdropTap: true);

    public void ShowDialog(string name, IDialogParameters parameters, DialogCallback callback)
        => _ = ShowInlineAsync(name, parameters, callback, closeOnBackdropTap: true);

    private async Task ShowInlineAsync(string name, IDialogParameters parameters, DialogCallback callback, bool closeOnBackdropTap)
    {
        var navPage = FindNavPage();
        if (navPage is null) return;

        if (parameters.TryGetValue<bool>(KnownDialogParameters.CloseOnBackdropTap, out var cb))
            closeOnBackdropTap = cb;

        var content = CreateContent(name, parameters);
        if (content is not Control ctrl) return;

        // Build close action: fires callback + pops modal
        Action<IDialogResult> closeAction = result =>
        {
            if (content is IDialogAware da) da.OnDialogClosed();
            callback.Invoke(result);
            if (navPage is INavigation n) _ = n.PopModalAsync();
        };

        var dialogPage = new DialogContainerPage(ctrl, closeAction, closeOnBackdropTap);

        // Inject IDialogCloser for AOT-safe ViewModel close
        parameters.Add(KnownDialogParameters.DialogCloser, (IDialogCloser)dialogPage);

        // Fire lifecycle on View if it implements IDialogAware
        if (content is IDialogAware viewAware)
            viewAware.OnDialogOpened(parameters);

        // Also fire on ViewModel (may fail silently if DataContext not set)
        if (content is StyledElement se)
        {
            System.Diagnostics.Debug.WriteLine($"[Dialog] DataContext={se.DataContext?.GetType().Name ?? "null"}");
            if (se.DataContext is IDialogAware vmAware)
                vmAware.OnDialogOpened(parameters);
            else
                System.Diagnostics.Debug.WriteLine("[Dialog] DataContext is not IDialogAware");
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
}
