using System.Drawing;
using JetBrains.Annotations;

namespace Radish.Windowing;

/// <summary>
/// A video mode supported by a <see cref="IDisplay"/>.
/// </summary>
/// <param name="Resolution">The resolution of the display.</param>
/// <param name="RefreshRate">The refresh rate of the display, in hertz.</param>
[PublicAPI]
public readonly record struct VideoMode(Size Resolution, float RefreshRate);
