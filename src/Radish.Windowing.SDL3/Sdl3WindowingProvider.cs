using JetBrains.Annotations;

namespace Radish.Windowing.SDL3;

/// <summary>
/// Windowing provider for SDL3.
/// Designed to be passed as a type parameter to <see cref="WindowingProvider.RegisterProvider"/>.
/// </summary>
[PublicAPI]
public sealed class Sdl3WindowingProvider : IWindowFactory
{
    /// <inheritdoc/>
    public static WindowFactoryDelegate WindowFactory 
        => static p => new Sdl3Window(p);
}