using Avalonia.Data.Converters;
using Avalonia.Media;
using System;

namespace SampleApp.Views;

public partial class ColorCardView : Avalonia.Controls.UserControl
{
    public static readonly IValueConverter ColorConverter = new ColorStringConverter();

    public ColorCardView()
    {
        Resources["ColorConverter"] = ColorConverter;
        InitializeComponent();
    }
}

internal class ColorStringConverter : IValueConverter
{
    public object? Convert(object? value, Type t, object? p, System.Globalization.CultureInfo c)
    {
        if (value is string s && Color.TryParse(s, out var color)) return new SolidColorBrush(color);
        return new SolidColorBrush(Colors.Gray);
    }

    public object? ConvertBack(object? value, Type t, object? p, System.Globalization.CultureInfo c) => throw new NotSupportedException();
}
