using Prism.Mvvm;

namespace Prism.Navigation;

/// <summary>
/// Accumulates <see cref="ViewRegistration"/> objects as views are registered.
/// Injected as a singleton so <see cref="NavigationRegistry"/> can consume them.
/// </summary>
public class ViewRegistrationCollection
{
    private readonly List<ViewRegistration> _registrations = new();

    /// <summary>
    /// Gets all collected registrations.
    /// </summary>
    public IEnumerable<ViewRegistration> Registrations => _registrations.AsReadOnly();

    /// <summary>
    /// Adds a registration.
    /// </summary>
    public void Add(ViewRegistration registration)
    {
        _registrations.Add(registration);
    }
}
