namespace Prism.Navigation;

/// <summary>
/// Provides a synchronous initialization method called before navigation
/// is completed.
/// </summary>
public interface IInitialize
{
    /// <summary>
    /// Initializes the page or ViewModel with the given navigation parameters.
    /// Called before the page is pushed onto the navigation stack.
    /// </summary>
    void Initialize(INavigationParameters parameters);
}

/// <summary>
/// Provides an asynchronous initialization method called before navigation
/// is completed.
/// </summary>
public interface IInitializeAsync
{
    /// <summary>
    /// Initializes the page or ViewModel with the given navigation parameters.
    /// Called before the page is pushed onto the navigation stack.
    /// </summary>
    Task InitializeAsync(INavigationParameters parameters);
}
