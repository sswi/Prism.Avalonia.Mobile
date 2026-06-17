using Prism.Mvvm;

namespace SampleApp.ViewModels;

public class ProfileViewModel : BindableBase
{
    public string UserName => "Prism User";
    public string UserEmail => "user@avalonia.mobile";
}
