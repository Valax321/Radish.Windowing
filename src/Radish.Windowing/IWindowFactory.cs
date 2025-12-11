using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// Static interface for classes providing a factory function for <see cref="IWindow"/> instances.
/// </summary>
[PublicAPI]
public interface IWindowFactory
{
    /// <summary>
    /// Factory function for creating windows.
    /// </summary>
    public static abstract WindowFactoryDelegate WindowFactory { get; }
}