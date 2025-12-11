using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Additional flags used by <see cref="IInputContext.BeginTextInput"/>
/// </summary>
[Flags, PublicAPI]
public enum TextInputFlags
{
    /// <summary>
    /// No additional behaviour.
    /// </summary>
    None = 0,
    /// <summary>
    /// Enable autocorrect on the system keyboard.
    /// </summary>
    EnableAutoCorrect = 1 << 0,
    /// <summary>
    /// Allow inputting multiple lines of text.
    /// </summary>
    /// <remarks>Should not be relied on for validating text input for you as the backend may ignore/not implement support for this.</remarks>
    MultilineTextInput = 1 << 1
}