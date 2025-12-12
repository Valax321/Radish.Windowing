namespace Radish.Windowing;

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when its resources are being initialised.
/// At this point the window's internal state is valid and can be accessed.
/// </summary>
public delegate void WindowLoadDelegate();

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when it is updating.
/// <param name="deltaTime">The time elapsed since the last update.</param>
/// </summary>
public delegate void WindowUpdateDelegate(TimeSpan deltaTime);

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when it is rendering a frame.
/// <param name="deltaTime">The time elapsed since the last frame.</param>
/// </summary>
public delegate void WindowRenderDelegate(TimeSpan deltaTime);

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when it wants to close.
/// <param name="args">Arguments for the close event. Allows cancelling the close request.</param>
/// </summary>
public delegate void WindowCloseRequestedDelegate(ref WindowClosingEventArgs args);

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when it is about to close.
/// </summary>
public delegate void WindowClosingDelegate();

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when it is moved.
/// </summary>
public delegate void WindowMovedDelegate();

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when it is resized.
/// </summary>
public delegate void WindowResizedDelegate();

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when its pixel size changes.
/// </summary>
public delegate void WindowPixelSizeChangedDelegate();

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when its display changes.
/// </summary>
public delegate void WindowDisplayChangedDelegate();

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when the window's focus state changes.
/// </summary>
public delegate void WindowFocusChangedDelegate();

/// <summary>
/// Delegate invoked on a <see cref="IWindow"/> when a display is added or removed from the desktop.
/// </summary>
public delegate void DisplaysChangedDelegate(IDisplay display);

/// <summary>
/// Signature for a window factory function provided by a <see cref="IWindowFactory"/>.
/// </summary>
public delegate IWindow WindowFactoryDelegate(WindowInitParameters initParams);
