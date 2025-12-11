namespace Radish.Windowing;

/// <summary>
/// Exception thrown when <see cref="WindowingProvider"/> has no registered provider and a window is created.
/// </summary>
/// <param name="message">The message describing the exception.</param>
public class WindowProviderNotFoundException(string message) : Exception(message);
