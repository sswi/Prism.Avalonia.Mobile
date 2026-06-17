using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;

namespace Prism.Dialogs;

/// <summary>
/// Hosts dialog content as centered card with backdrop.
/// Tapping backdrop triggers close via the provided <see cref="Action{IDialogResult}"/>.
/// Always set <see cref="CloseAction"/> before pushing to modal stack.
/// </summary>
public class DialogContainerPage : ContentPage, IDialogCloser
{
    private readonly bool _closeOnBackdropTap;
    private Action<IDialogResult>? _closeAction;

    /// <summary>
    /// Sets the action invoked to close the dialog. Called by DialogService
    /// before pushing the modal.
    /// </summary>
    public Action<IDialogResult>? CloseAction
    {
        get => _closeAction;
        set
        {
            _closeAction = value;
            // Also inject self as IDialogCloser into dialog parameters
        }
    }

    public DialogContainerPage(Control dialogContent, Action<IDialogResult> closeAction, bool closeOnBackdropTap = true)
    {
        _closeAction = closeAction;
        _closeOnBackdropTap = closeOnBackdropTap;
        Background = Brushes.Transparent;

        var backdrop = new Border
        {
            Background = new SolidColorBrush(Color.Parse("#80000000")),
            Child = new Panel
            {
                Children =
                {
                    new Border
                    {
                        MaxWidth = 420, MaxHeight = 500,
                        Background = Brushes.White,
                        CornerRadius = new CornerRadius(12),
                        BoxShadow = new BoxShadows(new BoxShadow
                            { OffsetX = 0, OffsetY = 8, Blur = 32, Color = Color.Parse("#40000000") }),
                        Child = dialogContent,
                        HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                        VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                        Margin = new Thickness(24)
                    }
                }
            }
        };

        if (_closeOnBackdropTap)
        {
            backdrop.PointerPressed += (s, e) =>
            {
                if (e.Source == backdrop)
                    _closeAction?.Invoke(new DialogResult { Result = ButtonResult.Cancel });
            };
        }

        Content = backdrop;
    }

    /// <inheritdoc />
    void IDialogCloser.Close(IDialogResult result) => _closeAction?.Invoke(result);
}
