using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// Registry for <see cref="IWindow"/> factory methods.
/// </summary>
[PublicAPI]
public static class WindowingProvider
{
    internal static WindowFactoryDelegate? WindowFactory { get; private set; }
    
    /// <summary>
    /// Registers a <see cref="IWindowFactory"/> type.
    /// </summary>
    /// <typeparam name="T">The type of window factory to register.</typeparam>
    public static void RegisterProvider<T>() where T : IWindowFactory
    {
        WindowFactory = T.WindowFactory;
    }

    /// <summary>
    /// Creates a new window instance.
    /// </summary>
    /// <param name="initParameters">Parameters to create the window with.</param>
    /// <returns>The created window object.</returns>
    /// <exception cref="WindowProviderNotFoundException">Thrown if a windowing provider has not yet been registered using <see cref="WindowingProvider.RegisterProvider"/>.</exception>
    public static IWindow CreateWindow(WindowInitParameters initParameters)
    {
        if (WindowFactory is null)
            throw new WindowProviderNotFoundException(
                "No window provider has been registered. Be sure to call WindowingProvider.RegisterProvider with a windowing provider type before creating a window.");

        return WindowFactory(initParameters);
    }
}