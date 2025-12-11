using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// Exception that is thrown by window providers when an internal/provider-specific error occurs.
/// </summary>
/// <param name="message">Description of the error.</param>
[PublicAPI]
public class NativeWindowException(string message) : Exception(message);
