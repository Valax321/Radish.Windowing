using System.Drawing;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3.InputDevices;

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
            return SDL_GetWindowTitle(NativeHandle);
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL_SetWindowTitle(NativeHandle, value))
                throw new NativeWindowException(SDL_GetError());
        }
    }

    /// <inheritdoc/>
    public bool IsVisible
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            var flags = SDL_GetWindowFlags(NativeHandle);
            return !flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_HIDDEN);
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (value)
                SDL_ShowWindow(NativeHandle);
            else
                SDL_HideWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public WindowState WindowState
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            var flags = SDL_GetWindowFlags(NativeHandle);
            if (flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_MINIMIZED))
                return WindowState.Minimized;
            if (flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_MAXIMIZED))
                return WindowState.Maximized;
            
            return WindowState.Normal;
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            switch (value)
            {
                case WindowState.Normal:
                    if (!SDL_RestoreWindow(NativeHandle))
                        throw new NativeWindowException(SDL_GetError());
                    break;
                case WindowState.Minimized:
                    if (!SDL_MinimizeWindow(NativeHandle))
                        throw new NativeWindowException(SDL_GetError());
                    break;
                case WindowState.Maximized:
                    if (!SDL_MaximizeWindow(NativeHandle))
                        throw new NativeWindowException(SDL_GetError());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            SDL_SyncWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public bool Resizable
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            var flags = SDL_GetWindowFlags(NativeHandle);
            return flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL_SetWindowResizable(NativeHandle, value))
                throw new NativeWindowException(SDL_GetError());
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
            return SDL_GetWindowPosition(NativeHandle, out var x, out var y)
                ? new Point(x, y)
                : throw new NativeWindowException(SDL_GetError());
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL_SetWindowPosition(NativeHandle, value.X, value.Y))
                throw new NativeWindowException(SDL_GetError());
            SDL_SyncWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public Size Size
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL_GetWindowSize(NativeHandle, out var w, out var h)
                ? new Size(w, h)
                : throw new NativeWindowException(SDL_GetError());
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL_SetWindowSize(NativeHandle, value.Width, value.Height))
                throw new NativeWindowException(SDL_GetError());
            SDL_SyncWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public Size PixelSize
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL_GetWindowSizeInPixels(NativeHandle, out var w, out var h)
                ? new Size(w, h)
                : throw new NativeWindowException(SDL_GetError());
        }
    }

    /// <inheritdoc />
    public Size MinimumSize
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL_GetWindowMinimumSize(NativeHandle, out var w, out var h)
                ? new Size(w, h)
                : throw new NativeWindowException(SDL_GetError());
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL_SetWindowMinimumSize(NativeHandle, value.Width, value.Height))
                throw new NativeWindowException(SDL_GetError());
        }
    }

    /// <inheritdoc />
    public Size MaximumSize
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL_GetWindowMaximumSize(NativeHandle, out var w, out var h)
                ? new Size(w, h)
                : throw new NativeWindowException(SDL_GetError());
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            if (!SDL_SetWindowMaximumSize(NativeHandle, value.Width, value.Height))
                throw new NativeWindowException(SDL_GetError());
        }    
    }

    /// <inheritdoc />
    public unsafe FullscreenMode FullscreenMode
    {
        get
        {
            ThrowIfNativeHandleInvalid();

            var dm = SDL_GetWindowFullscreenMode(NativeHandle);
            if (dm != null)
                return FullscreenMode.Exclusive;

            var flags = SDL_GetWindowFlags(NativeHandle);
            return flags.HasFlag(SDL_WindowFlags.SDL_WINDOW_FULLSCREEN) 
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
                    if (!SDL_SetWindowFullscreen(NativeHandle, false))
                        throw new NativeWindowException(SDL_GetError());
                    break;
                case FullscreenMode.Desktop:
                    if (!SDL_SetWindowFullscreen(NativeHandle, true))
                        throw new NativeWindowException(SDL_GetError());
                    break;
                case FullscreenMode.Exclusive:
                    // Moving into fullscreen without actually specifying a video mode
                    // is essentially just a best guess of what the user wants to do.
                    // Try and keep the current resolution but use the desktop refresh rate.
                    var sz = Size;
                    var r = CurrentDisplay.NativeVideoMode.RefreshRate;
                    var success = SDL_GetClosestFullscreenDisplayMode((uint)CurrentDisplay.NativeHandle, sz.Width, sz.Height,
                        r, true, out var dm);
                    if (!success || !SDL_SetWindowFullscreenMode(NativeHandle, ref dm))
                        throw new NativeWindowException(SDL_GetError());
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }

            SDL_SyncWindow(NativeHandle);
        }
    }

    /// <inheritdoc />
    public float ContentScale
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return SDL_GetWindowDisplayScale(NativeHandle);
        }
    }

    /// <inheritdoc />
    public unsafe IDisplay CurrentDisplay
    {
        get
        {
            ThrowIfNativeHandleInvalid();
            return new SdlDisplay(SDL_GetDisplayForWindow(NativeHandle));
        }
        set
        {
            ThrowIfNativeHandleInvalid();
            var d = (SdlDisplay)value;

            if (FullscreenMode == FullscreenMode.Exclusive)
            {
                var dm = SDL_GetCurrentDisplayMode(d.DisplayIndex);
                if (dm == null)
                    throw new NativeWindowException(SDL_GetError());

                if (!SDL_SetWindowFullscreenMode(NativeHandle, ref Unsafe.AsRef<SDL_DisplayMode>(dm)))
                    throw new NativeWindowException(SDL_GetError());
                
                SDL_SyncWindow(NativeHandle);
            }
            else
            {
                // Just centre it.
                SDL_SetWindowPosition(NativeHandle, 
                    (int)SDL_WindowPosCenteredDisplay(d.DisplayIndex),
                    (int)SDL_WindowPosCenteredDisplay(d.DisplayIndex)
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
            var d = SDL_GetPrimaryDisplay();
            if (d == 0)
                throw new NativeWindowException(SDL_GetError());

            return new SdlDisplay(d);
        }
    }

    /// <inheritdoc />
    public unsafe IEnumerable<IDisplay> Displays
    {
        get
        {
            ThrowIfNativeHandleInvalid();

            var displays = (uint*)SDL_GetDisplays(out var cnt);
            if (displays == null)
                throw new NativeWindowException(SDL_GetError());

            var displayArr = new SdlDisplay[cnt];

            try
            {
                for (var i = 0; i < cnt; ++i)
                {
                    var dm = displays[i];
                    displayArr[i] = new SdlDisplay(dm);
                }
            }
            finally
            {
                SDL_free((nint)displays);
            }

            return displayArr;
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
    /// Event invoked for every <see cref="SDL_Event"/> being processed in the main loop.
    /// Note that is callback is invoked AFTER all window-internal event processing has occurred.
    /// </summary>
    public event EventHandlerDelegate? EventProcess;

    internal event Action? PreEventProcess;
    internal event Action? PostEventProcess;

    private readonly WindowInitParameters _initParameters;
    private bool _closeRequested;
    private ulong _lastUpdateTicks;
    
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
                if (!SDL_SetWindowFullscreen(NativeHandle, false))
                    return false;
                if (!SDL_SetWindowPosition(NativeHandle,
                        (int)SDL_WindowPosCenteredDisplay(d.DisplayIndex),
                        (int)SDL_WindowPosCenteredDisplay(d.DisplayIndex)
                    )) return false;
                if (!SDL_SetWindowSize(NativeHandle, videoMode.Resolution.Width, videoMode.Resolution.Height))
                    return false;
                break;
            
            case FullscreenMode.Desktop:
                if (!SDL_SetWindowFullscreen(NativeHandle, true))
                    return false;
                if (!SDL_SetWindowPosition(NativeHandle,
                        (int)SDL_WindowPosUndefinedDisplay(d.DisplayIndex),
                        (int)SDL_WindowPosUndefinedDisplay(d.DisplayIndex)
                    )) return false;
                break;
            
            case FullscreenMode.Exclusive:
                if (!SDL_GetClosestFullscreenDisplayMode(d.DisplayIndex, videoMode.Resolution.Width,
                        videoMode.Resolution.Height, videoMode.RefreshRate, true, out var dm))
                    return false;
                SDL_SetWindowFullscreenMode(NativeHandle, ref dm);
                break;
            
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        SDL_SyncWindow(NativeHandle);
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
        
        if (!SDL_InitSubSystem(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS | SDL_InitFlags.SDL_INIT_GAMEPAD))
            throw new NativeWindowException(SDL_GetError());
        
        AppDomain.CurrentDomain.ProcessExit += (_, _) =>
        {
            SDL_QuitSubSystem(SDL_InitFlags.SDL_INIT_VIDEO | SDL_InitFlags.SDL_INIT_EVENTS | SDL_InitFlags.SDL_INIT_GAMEPAD);
            SDL_Quit();
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
        var flags = SDL_WindowFlags.SDL_WINDOW_HIGH_PIXEL_DENSITY;
        if (_initParameters.Hidden)
            flags |= SDL_WindowFlags.SDL_WINDOW_HIDDEN;

        if (_initParameters.Resizable)
            flags |= SDL_WindowFlags.SDL_WINDOW_RESIZABLE;

        switch (backendParams.WindowType)
        {
            case SpecializedWindowType.None:
                break;
            case SpecializedWindowType.OpenGLCompatible:
                flags |= SDL_WindowFlags.SDL_WINDOW_OPENGL;
                break;
            case SpecializedWindowType.VulkanCompatible:
                flags |= SDL_WindowFlags.SDL_WINDOW_VULKAN;
                break;
            case SpecializedWindowType.MetalCompatible:
                flags |= SDL_WindowFlags.SDL_WINDOW_METAL;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // Create it now
        NativeHandle = SDL_CreateWindow(_initParameters.Title!, 
            _initParameters.Size.Width, _initParameters.Size.Height,
            flags);

        // Error check
        if (NativeHandle == IntPtr.Zero)
            throw new NativeWindowException(SDL_GetError());
    }
    
    #endregion
    
    #region Main Loop

    private void RunMainLoop()
    {
        _lastUpdateTicks = SDL_GetPerformanceCounter();
        
        while (!_closeRequested)
        {
            PollEvents();

            var deltaTime = CalculateElapsedTime();
            
            // Unlike Silk.NET, we leave it up to the user to implement a fixed time step if they want one.
            Update?.Invoke(deltaTime);
            Render?.Invoke(deltaTime);
        }
    }

    private TimeSpan CalculateElapsedTime()
    {
        var t = SDL_GetPerformanceCounter();
        var tDiff = t - _lastUpdateTicks;
        _lastUpdateTicks = t;

        var diffSeconds = tDiff / (double)SDL_GetPerformanceFrequency();
        return TimeSpan.FromSeconds(diffSeconds);
    }

    private void PollEvents()
    {
        PreEventProcess?.Invoke();
        while (SDL_PollEvent(out var @event))
        {
            switch ((SDL_EventType)@event.type)
            {
                // Process should quit
                case SDL_EventType.SDL_EVENT_QUIT:
                    ProcessQuitEvent(false);
                    break;
                
                // Display add/removed
                case SDL_EventType.SDL_EVENT_DISPLAY_ADDED:
                    DisplayAdded?.Invoke(new SdlDisplay(@event.display.displayID));
                    break;
                case SDL_EventType.SDL_EVENT_DISPLAY_REMOVED:
                    DisplayRemoved?.Invoke(new SdlDisplay(@event.display.displayID));
                    break;
                
                case SDL_EventType.SDL_EVENT_WINDOW_MOVED:
                    Moved?.Invoke();
                    break;
                case SDL_EventType.SDL_EVENT_WINDOW_RESIZED:
                    Resized?.Invoke();
                    break;
                case SDL_EventType.SDL_EVENT_WINDOW_PIXEL_SIZE_CHANGED:
                    PixelSizeChanged?.Invoke();
                    break;
                case SDL_EventType.SDL_EVENT_WINDOW_DISPLAY_CHANGED:
                    DisplayChanged?.Invoke();
                    break;
                
                // Focus events
                case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_GAINED:
                    Focused = true;
                    GainedFocus?.Invoke();
                    break;
                case SDL_EventType.SDL_EVENT_WINDOW_FOCUS_LOST:
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
            SDL_DestroyWindow(NativeHandle);
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
