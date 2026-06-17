using Android.App;
using Android.Runtime;
using Avalonia;
using Avalonia.Android;

namespace SampleApp.Android;

[Application]
public class Application : AvaloniaAndroidApplication<SampleApp.App>
{
    public Application(nint javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
    {
    }

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .LogToTrace();
    }
}
