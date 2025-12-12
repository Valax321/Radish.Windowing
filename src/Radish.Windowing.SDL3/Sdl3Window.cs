using System.Drawing;
using JetBrains.Annotations;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.InputDevices;
using SDL3;

namespace Radish.Windowing.SDL3;

/// <summary>
/// SDL3 implementation of the window interface.
/// </summary>
[PublicAPI]
public class Sdl3Window : IWindow
{
    /// <inheritdoc/>
    /// <remarks>For SDL3, this is an SDL_Window pointer.</remarks>
    public IntPtr NativeHandle { get; private set; }

    /// <inheritdoc/>
    public string Title
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL.GetWindowTitle(NativeHandle);
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL.SetWindowTitle(NativeHandle, value))
                throw new NativeWindowException(SDL.GetError());
        }
    }

    /// <inheritdoc/>
    public bool IsVisible
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            var flags = SDL.GetWindowFlags(NativeHandle);
            return !flags.HasFlag(SDL.WindowFlags.Hidden);
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (value)
                SDL.ShowWindow(NativeHandle);
            else
                SDL.HideWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public WindowState WindowState
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            var flags = SDL.GetWindowFlags(NativeHandle);
            if (flags.HasFlag(SDL.WindowFlags.Minimized))
                return WindowState.Minimized;
            if (flags.HasFlag(SDL.WindowFlags.Maximized))
                return WindowState.Maximized;
            
            return WindowState.Normal;
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            switch (value)
            {
                case WindowState.Normal:
                    if (!SDL.RestoreWindow(NativeHandle))
                        throw new NativeWindowException(SDL.GetError());
                    break;
                case WindowState.Minimized:
                    if (!SDL.MinimizeWindow(NativeHandle))
                        throw new NativeWindowException(SDL.GetError());
                    break;
                case WindowState.Maximized:
                    if (!SDL.MaximizeWindow(NativeHandle))
                        throw new NativeWindowException(SDL.GetError());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            SDL.SyncWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public bool Resizable
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            var flags = SDL.GetWindowFlags(NativeHandle);
            return flags.HasFlag(SDL.WindowFlags.Resizable);
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL.SetWindowResizable(NativeHandle, value))
                throw new NativeWindowException(SDL.GetError());
        }
    }

    /// <inheritdoc />
    public bool Focused { get; private set; }

    /// <inheritdoc />
    public Point Position
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL.GetWindowPosition(NativeHandle, out var x, out var y)
                ? new Point(x, y)
                : throw new NativeWindowException(SDL.GetError());
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL.SetWindowPosition(NativeHandle, value.X, value.Y))
                throw new NativeWindowException(SDL.GetError());
            SDL.SyncWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public Size Size
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL.GetWindowSize(NativeHandle, out var w, out var h)
                ? new Size(w, h)
                : throw new NativeWindowException(SDL.GetError());
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL.SetWindowSize(NativeHandle, value.Width, value.Height))
                throw new NativeWindowException(SDL.GetError());
            SDL.SyncWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public Size PixelSize
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL.GetWindowSizeInPixels(NativeHandle, out var w, out var h)
                ? new Size(w, h)
                : throw new NativeWindowException(SDL.GetError());
        }
    }

    /// <inheritdoc />
    public Size MinimumSize
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL.GetWindowMinimumSize(NativeHandle, out var w, out var h)
                ? new Size(w, h)
                : throw new NativeWindowException(SDL.GetError());
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL.SetWindowMinimumSize(NativeHandle, value.Width, value.Height))
                throw new NativeWindowException(SDL.GetError());
        }
    }

    /// <inheritdoc />
    public Size MaximumSize
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL.GetWindowMaximumSize(NativeHandle, out var w, out var h)
                ? new Size(w, h)
                : throw new NativeWindowException(SDL.GetError());
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL.SetWindowMaximumSize(NativeHandle, value.Width, value.Height))
                throw new NativeWindowException(SDL.GetError());
        }    
    }

    /// <inheritdoc />
    public FullscreenMode FullscreenMode
    {
        get
        {
            ThrowIfNativeHandleInvalid();

            var dm = SDL.GetWindowFullscreenMode(NativeHandle);
            if (dm != null)
                return FullscreenMode.Exclusive;

            var flags = SDL.GetWindowFlags(NativeHandle);
            return flags.HasFlag(SDL.WindowFlags.Fullscreen) 
                ? FullscreenMode.Desktop 
                : FullscreenMode.Windowed;
        }
        set
        {
            ThrowIfNativeHandleInvalid();

            if (value == FullscreenMode)
                return;

            switch (value)
            {
                case FullscreenMode.Windowed:
                    if (!SDL.SetWindowFullscreen(NativeHandle, false))
                        throw new NativeWindowException(SDL.GetError());
                    break;
                case FullscreenMode.Desktop:
                    if (!SDL.SetWindowFullscreenMode(NativeHandle, IntPtr.Zero))
                        throw new NativeWindowException(SDL.GetError());
                    break;
                case FullscreenMode.Exclusive:
                    // Moving into fullscreen without actually specifying a video mode
                    // is essentially just a best guess of what the user wants to do.
                    // Try and keep the current resolution but use the desktop refresh rate.
                    var sz = Size;
                    var r = CurrentDisplay.NativeVideoMode.RefreshRate;
                    var success = SDL.GetClosestFullscreenDisplayMode((uint)CurrentDisplay.NativeHandle, sz.Width, sz.Height,
                        r, true, out var dm);
                    if (!success || !SDL.SetWindowFullscreenMode(NativeHandle, dm))
                        throw new NativeWindowException(SDL.GetError());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            SDL.SyncWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public float ContentScale
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL.GetWindowDisplayScale(NativeHandle);
        }
    }

    /// <inheritdoc />
    public IDisplay CurrentDisplay
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return new SdlDisplay(SDL.GetDisplayForWindow(NativeHandle));
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            var d = (SdlDisplay)value;

            if (FullscreenMode == FullscreenMode.Exclusive)
            {
                var dm = SDL.GetCurrentDisplayMode(d.DisplayIndex);
                if (!dm.HasValue)
                    throw new NativeWindowException(SDL.GetError());

                if (!SDL.SetWindowFullscreenMode(NativeHandle, dm.Value))
                    throw new NativeWindowException(SDL.GetError());
                
                SDL.SyncWindow(NativeHandle);
            }
            else
            {
                // Just centre it.
                SDL.SetWindowPosition(NativeHandle, 
                    (int)SDL.WindowPosCenteredDisplay((int)d.DisplayIndex),
                    (int)SDL.WindowPosCenteredDisplay((int)d.DisplayIndex)
                );
            }
        }
    }

    /// <inheritdoc />
    public IDisplay PrimaryDisplay
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            var d = SDL.GetPrimaryDisplay();
            if (d == 0)
                throw new NativeWindowException(SDL.GetError());

            return new SdlDisplay(d);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IDisplay> Displays
    {
        get
        {
            ThrowIfNativeHandleInvalid();

            var displays = SDL.GetDisplays(out _);
            if (displays == null)
                throw new NativeWindowException(SDL.GetError());

            foreach (var d in displays)
                yield return new SdlDisplay(d);
        }
    }

    /// <inheritdoc/>
    public event WindowLoadDelegate? Loaded;
    
    /// <inheritdoc/>
    public event WindowUpdateDelegate? Update;
    
    /// <inheritdoc/>
    public event WindowRenderDelegate? Render;
    
    /// <inheritdoc/>
    public event WindowCloseRequestedDelegate? CloseRequested;

    /// <inheritdoc/>
    public event WindowClosingDelegate? Closing;

    /// <inheritdoc />
    public event WindowMovedDelegate? Moved;

    /// <inheritdoc />
    public event WindowResizedDelegate? Resized;

    /// <inheritdoc />
    public event WindowPixelSizeChangedDelegate? PixelSizeChanged;

    /// <inheritdoc />
    public event WindowFocusChangedDelegate? GainedFocus;

    /// <inheritdoc />
    public event WindowFocusChangedDelegate? LostFocus;

    /// <inheritdoc />
    public event WindowDisplayChangedDelegate? DisplayChanged;

    /// <inheritdoc />
    public event DisplaysChangedDelegate? DisplayAdded;

    /// <inheritdoc />
    public event DisplaysChangedDelegate? DisplayRemoved;

    /// <summary>
    /// Event invoked for every <see cref="SDL.Event"/> being processed in the main loop.
    /// Note that is callback is invoked AFTER all window-internal event processing has occurred.
    /// </summary>
    public event EventHandlerDelegate? EventProcess;

    internal event Action? PreEventProcess;
    internal event Action? PostEventProcess;

    private readonly WindowInitParameters _initParameters;
    private bool _closeRequested;
    
    /// <inheritdoc/>
    public void Run()
    {
        CreateWindowInternal();
        Loaded?.Invoke();
        RunMainLoop();
        Closing?.Invoke();
    }

    /// <inheritdoc />
    public void Close(bool force = false) => ProcessQuitEvent(force);
    
    #region Video Modes

    /// <inheritdoc />
    public bool MoveToDisplay(IDisplay display, FullscreenMode mode, VideoMode videoMode)
    {
        ThrowIfNativeHandleInvalid();

        var d = (SdlDisplay)display;
        
        switch (mode)
        {
            case FullscreenMode.Windowed:
                if (!SDL.SetWindowFullscreen(NativeHandle, false))
                    return false;
                if (!SDL.SetWindowPosition(NativeHandle,
                        (int)SDL.WindowPosCenteredDisplay((int)d.DisplayIndex),
                        (int)SDL.WindowPosCenteredDisplay((int)d.DisplayIndex)
                    )) return false;
                if (!SDL.SetWindowSize(NativeHandle, videoMode.Resolution.Width, videoMode.Resolution.Height))
                    return false;
                break;
            
            case FullscreenMode.Desktop:
                if (!SDL.SetWindowFullscreenMode(NativeHandle, IntPtr.Zero))
                    return false;
                if (!SDL.SetWindowPosition(NativeHandle,
                        (int)SDL.WindowPosUndefinedDisplay((int)d.DisplayIndex),
                        (int)SDL.WindowPosUndefinedDisplay((int)d.DisplayIndex)
                    )) return false;
                break;
            
            case FullscreenMode.Exclusive:
                if (!SDL.GetClosestFullscreenDisplayMode(d.DisplayIndex, videoMode.Resolution.Width,
                        videoMode.Resolution.Height, videoMode.RefreshRate, true, out var dm))
                    return false;
                SDL.SetWindowFullscreenMode(NativeHandle, dm);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        SDL.SyncWindow(NativeHandle);
        return true;
    }
    
    #endregion

    #region Initialisation
    
    /// <inheritdoc/>
    public IInputContext CreateInput()
    {
        ThrowIfNativeHandleInvalid();
        return new Sdl3InputContext(this);
    }

    static Sdl3Window()
    {
        // Bring up SDL and tear it down when we're unloading.
        
        if (!SDL.InitSubSystem(SDL.InitFlags.Video | SDL.InitFlags.Events | SDL.InitFlags.Gamepad))
            throw new NativeWindowException(SDL.GetError());
        
        AppDomain.CurrentDomain.DomainUnload += (_, _) =>
        {
            SDL.QuitSubSystem(SDL.InitFlags.Video | SDL.InitFlags.Events | SDL.InitFlags.Gamepad);
            SDL.Quit();
        };
    }
    
    internal Sdl3Window(WindowInitParameters initParams)
    {
        _initParameters = initParams;
        _initParameters.Title ??= AppDomain.CurrentDomain.FriendlyName;
    }

    private void CreateWindowInternal()
    {
        // Get the passed backend params or create reasonable default values
        Sdl3BackendWindowParameters backendParams;
        if (_initParameters.BackendParameters is Sdl3BackendWindowParameters wp)
            backendParams = wp;
        else
            backendParams = new Sdl3BackendWindowParameters(); // Defaults are sensible here
        
        // Convert initialisation data into window flags
        var flags = SDL.WindowFlags.HighPixelDensity;
        if (_initParameters.Hidden)
            flags |= SDL.WindowFlags.Hidden;

        if (_initParameters.Resizable)
            flags |= SDL.WindowFlags.Resizable;

        switch (backendParams.WindowType)
        {
            case SpecializedWindowType.None:
                break;
            case SpecializedWindowType.OpenGLCompatible:
                flags |= SDL.WindowFlags.OpenGL;
                break;
            case SpecializedWindowType.VulkanCompatible:
                flags |= SDL.WindowFlags.Vulkan;
                break;
            case SpecializedWindowType.MetalCompatible:
                flags |= SDL.WindowFlags.Metal;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Create it now
        NativeHandle = SDL.CreateWindow(_initParameters.Title!, 
            _initParameters.Size.Width, _initParameters.Size.Height,
            flags);

        // Error check
        if (NativeHandle == IntPtr.Zero)
            throw new NativeWindowException(SDL.GetError());
    }
    
    #endregion
    
    #region Main Loop

    private void RunMainLoop()
    {
        while (!_closeRequested)
        {
            PollEvents();
            Update?.Invoke(TimeSpan.Zero);
            Render?.Invoke(TimeSpan.Zero);
        }
    }

    private void PollEvents()
    {
        PreEventProcess?.Invoke();
        while (SDL.PollEvent(out var @event))
        {
            switch ((SDL.EventType)@event.Type)
            {
                // Process should quit
                case SDL.EventType.Quit:
                    ProcessQuitEvent(false);
                    break;
                
                // Display add/removed
                case SDL.EventType.DisplayAdded:
                    DisplayAdded?.Invoke(new SdlDisplay(@event.Display.DisplayID));
                    break;
                case SDL.EventType.DisplayRemoved:
                    DisplayRemoved?.Invoke(new SdlDisplay(@event.Display.DisplayID));
                    break;
                
                case SDL.EventType.WindowMoved:
                    Moved?.Invoke();
                    break;
                case SDL.EventType.WindowResized:
                    Resized?.Invoke();
                    break;
                case SDL.EventType.WindowPixelSizeChanged:
                    PixelSizeChanged?.Invoke();
                    break;
                case SDL.EventType.WindowDisplayChanged:
                    DisplayChanged?.Invoke();
                    break;
                
                // Focus events
                case SDL.EventType.WindowFocusGained:
                    Focused = true;
                    GainedFocus?.Invoke();
                    break;
                case SDL.EventType.WindowFocusLost:
                    Focused = false;
                    LostFocus?.Invoke();
                    break;
            }
            
            EventProcess?.Invoke(in @event);
        }
        PostEventProcess?.Invoke();
    }

    private void ProcessQuitEvent(bool skipEventCallback)
    {
        if (skipEventCallback)
        {
            _closeRequested = true;
            return;
        }
        
        var args = new WindowClosingEventArgs { ShouldClose = true };
        
        CloseRequested?.Invoke(ref args);

        if (args.ShouldClose)
            _closeRequested = true;
    }
    
    #endregion

    #region Shutdown/Cleanup
    
    /// <summary>
    /// Destructor for the class. Only here in case you forget to call <see cref="Dispose"/>.
    /// </summary>
    ~Sdl3Window() => Dispose();

    /// <inheritdoc/>
    /// <remarks>Be sure to call the base constructor if you override this or else your window will leak memory!</remarks>
    public virtual void Dispose()
    {
        GC.SuppressFinalize(this);
        if (NativeHandle != IntPtr.Zero)
        {
            SDL.DestroyWindow(NativeHandle);
            NativeHandle = IntPtr.Zero;
        }
    }
    
    #endregion
    
    #region Utility Methods
    
    internal void ThrowIfNativeHandleInvalid()
    {
        if (NativeHandle == IntPtr.Zero)
            throw new NativeWindowException("Window is not yet initialized or has been disposed");
    }
    
    #endregion
}
