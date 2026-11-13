// ReSharper disable InconsistentNaming

using JetBrains.Annotations;

#pragma warning disable IDE0130 // Namespace does not match folder

namespace SDL3;

[PublicAPI]
partial class SDL
{
    #region Window position masks

    /*
     * SDL3-CS doesn't really re-implement SDL's macros, but these are useful to have.
     */

    /// <summary>
    /// https://wiki.libsdl.org/SDL3/SDL_WINDOWPOS_CENTERED_MASK
    /// </summary>
    public const uint SDL_WINDOWPOS_CENTERED_MASK = 0x2FFF0000u;

    /// <summary>
    /// https://wiki.libsdl.org/SDL3/SDL_WINDOWPOS_UNDEFINED_MASK
    /// </summary>
    public const uint SDL_WINDOWPOS_UNDEFINED_MASK = 0x1FFF0000u;

    /// <summary>
    /// https://wiki.libsdl.org/SDL3/SDL_WINDOWPOS_CENTERED_DISPLAY
    /// </summary>
    public static uint SDL_WindowPosCenteredDisplay(uint display) => SDL_WINDOWPOS_CENTERED_MASK | display;

    /// <summary>
    /// https://wiki.libsdl.org/SDL3/SDL_WINDOWPOS_CENTERED
    /// </summary>
    public static uint SDL_WindowPosCentered() => SDL_WindowPosCenteredDisplay(0);

    /// <summary>
    /// https://wiki.libsdl.org/SDL3/SDL_WINDOWPOS_UNDEFINED_DISPLAY
    /// </summary>
    public static uint SDL_WindowPosUndefinedDisplay(uint display) => SDL_WINDOWPOS_UNDEFINED_MASK | display;

    /// <summary>
    /// https://wiki.libsdl.org/SDL3/SDL_WINDOWPOS_UNDEFINED
    /// </summary>
    public static uint SDL_WindowPosUndefined() => SDL_WindowPosUndefinedDisplay(0);

    #endregion

    /// <summary>
    /// https://wiki.libsdl.org/SDL3/SDL_TOUCH_MOUSEID
    /// </summary>
    public const uint SDL_TOUCH_MOUSEID = unchecked((uint)-1);
}
