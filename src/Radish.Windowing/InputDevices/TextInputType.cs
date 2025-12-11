namespace Radish.Windowing.InputDevices;

/// <summary>
/// Values describing what kind of text input an <see cref="IInputContext"/> can receive.
/// </summary>
public enum TextInputType
{
    /*
     * This is based off https://wiki.libsdl.org/SDL3/SDL_TextInputType
     * without the password visible options
     */
    
    /// <summary>
    /// Regular text.
    /// </summary>
    Text,
    /// <summary>
    /// A person's name.
    /// </summary>
    Name,
    /// <summary>
    /// An email address.
    /// </summary>
    Email,
    /// <summary>
    /// A username.
    /// </summary>
    Username,
    /// <summary>
    /// A password. Will be shown as **** hidden characters by the system keyboard (if backend uses one).
    /// </summary>
    Password,
    /// <summary>
    /// A numeric-only string of text.
    /// </summary>
    Number,
    /// <summary>
    /// A secure numeric-only string of text. Will be shown as **** hidden characters by the system keyboard (if backend uses one).
    /// </summary>
    Pin
}