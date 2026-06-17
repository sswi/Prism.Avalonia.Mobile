using Prism;

namespace SampleApp.Views;

[PrismView("EditPage", ViewModel = typeof(ViewModels.EditViewModel))]
public partial class EditPage : Avalonia.Controls.ContentPage
{
    public EditPage() => InitializeComponent();
}
