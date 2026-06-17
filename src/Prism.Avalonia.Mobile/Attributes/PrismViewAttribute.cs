namespace Prism;

/// <summary>
/// Marks a class as a Prism navigation view. The source generator will
/// produce compile-time factory methods for views marked with this attribute,
/// eliminating the need for <c>Activator.CreateInstance</c> or runtime
/// <c>Type.GetType</c> calls.
/// </summary>
/// <remarks>
/// <para>For AOT-compatible applications, use this attribute on all navigation
/// views in addition to calling <c>RegisterForNavigation&lt;T&gt;()</c>.</para>
///
/// <para>Example:</para>
/// <code>
/// [PrismView("MainPage", ViewModel = typeof(MainViewModel))]
/// public partial class MainPage : ContentPage
/// {
///     // ...
/// }
/// </code>
/// </remarks>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class PrismViewAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of <see cref="PrismViewAttribute"/>.
    /// </summary>
    /// <param name="navigationName">
    /// The navigation name. If not provided, the class name is used.
    /// </param>
    public PrismViewAttribute(string? navigationName = null)
    {
        NavigationName = navigationName;
    }

    /// <summary>
    /// Gets the navigation name for this view.
    /// </summary>
    public string? NavigationName { get; }

    /// <summary>
    /// Gets or sets the ViewModel type associated with this view.
    /// </summary>
    public Type? ViewModel { get; set; }

    /// <summary>
    /// Gets or sets the view type (Page, Region, or Dialog). Default is Page.
    /// </summary>
    public Mvvm.ViewType ViewType { get; set; } = Mvvm.ViewType.Page;
}
