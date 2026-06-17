namespace Prism.Navigation;

/// <summary>
/// Provides a synchronous navigation guard: implement to veto navigation
/// before it occurs.
/// </summary>
public interface IConfirmNavigation
{
    /// <summary>
    /// Determines whether navigation can proceed.
    /// </summary>
    /// <returns><c>true</c> to allow navigation; <c>false</c> to block it.</returns>
    bool CanNavigate(INavigationParameters parameters);
}

/// <summary>
/// Provides an asynchronous navigation guard: implement to veto navigation
/// before it occurs.
/// </summary>
public interface IConfirmNavigationAsync
{
    /// <summary>
    /// Determines whether navigation can proceed.
    /// </summary>
    /// <returns><c>true</c> to allow navigation; <c>false</c> to block it.</returns>
    Task<bool> CanNavigateAsync(INavigationParameters parameters);
}
