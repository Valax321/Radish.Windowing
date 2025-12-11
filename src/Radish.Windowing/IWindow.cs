using JetBrains.Annotations;
using Radish.Windowing.InputDevices;

namespace Radish.Windowing;

/// <summary>
/// Interface to a window.
/// </summary>
[PublicAPI]
public interface IWindow : IDisposable
{
    /// <summary>
    /// Handle to the backend's native window pointer. Can be used for direct access.
    /// </summary>
    public IntPtr NativeHandle { get; }

    /// <summary>
    /// The string displayed on the window title bar.
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// If true the window is visible on the user's display.
    /// </summary>
    public bool IsVisible { get; set; }

    /// <summary>
    /// Invoked when the window has finished initialising and its internal state can be safely accessed.
    /// </summary>
    public event WindowLoadDelegate Loaded;

    /// <summary>
    /// Invoked when the window is updating.
    /// </summary>
    public event WindowUpdateDelegate Update;

    /// <summary>
    /// Invoked when the window is rendering a frame.
    /// </summary>
    public event WindowRenderDelegate Render;

    /// <summary>
    /// Invoked when the window wants to close.
    /// </summary>
    public event WindowCloseRequestedDelegate CloseRequested;

    /// <summary>
    /// Invoked when the window is about to close.
    /// </summary>
    public event WindowClosingDelegate Closing;

    /// <summary>
    /// Creates an input context for the given window.
    /// </summary>
    /// <returns></returns>
    public IInputContext CreateInput();

    /// <summary>
    /// Runs the main loop for the window.
    /// Before this is called, none of the internal window state is valid and accessing properties can result in undefined behaviour.
    /// </summary>
    public void Run();

    /// <summary>
    /// Request to close the window.
    /// </summary>
    /// <param name="force">If <see langword="true"/> then the <see cref="CloseRequested"/> callback will be skipped.</param>
    public void Close(bool force = false);
}
