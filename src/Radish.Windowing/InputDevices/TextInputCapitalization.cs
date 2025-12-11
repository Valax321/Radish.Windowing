using JetBrains.Annotations;

namespace Radish.Windowing.InputDevices;

/// <summary>
/// Values describing how text should be capitalised by <see cref="IInputContext"/> keyboard input.
/// </summary>
/// <remarks>These should not be used in place of validating the text input manually as these modes may not be supported/implemented by the input backend.</remarks>
[PublicAPI]
public enum TextInputCapitalization
{
    /**
     * Directly mirrors https://wiki.libsdl.org/SDL3/SDL_Capitalization
     */
    
    /// <summary>
    /// No capitalisation applied.
    /// </summary>
    None,
    /// <summary>
    /// Capitalise first letter of sentences.
    /// </summary>
    Sentences,
    /// <summary>
    /// Capitalise first letter of each word.
    /// </summary>
    Words,
    /// <summary>
    /// Capitalise every letter.
    /// </summary>
    Letters
}