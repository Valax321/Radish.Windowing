using JetBrains.Annotations;

namespace Radish.Windowing.SDL3;

/// <summary>
/// SDL3-specific window properties.
/// </summary>
/// <param name="WindowType">Special window type that may be needed by some graphics APIs.</param>
[PublicAPI]
public record struct Sdl3BackendWindowParameters(
    SpecializedWindowType WindowType = SpecializedWindowType.None
) : IBackendWindowParameters;
