namespace Prism.Navigation;

/// <summary>
/// Well-known internal navigation parameters.
/// </summary>
internal static class KnownInternalParameters
{
    /// <summary>
    /// The navigation mode for the current operation.
    /// </summary>
    public const string NavigationMode = "__NavigationMode";
}

/// <summary>
/// Specifies the direction of navigation.
/// </summary>
internal enum NavigationMode
{
    /// <summary>Forward navigation to a new page.</summary>
    New,

    /// <summary>Backward navigation (going back).</summary>
    Back,
}
