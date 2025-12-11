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
            SDL.SetWindowTitle(NativeHandle, value);
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

    /// <summary>
    /// Event invoked for every <see cref="SDL.Event"/> being processed in the main loop.
    /// Note that is callback is invoked AFTER all window-internal event processing has occurred.
    /// </summary>
    public event EventHandlerDelegate? EventProcess;

    /// <inheritdoc/>
    public IInputContext CreateInput()
    {
        ThrowIfNativeHandleInvalid();
        return new Sdl3InputContext(this);
    }

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

    #region Initialisation

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
        while (SDL.PollEvent(out var @event))
        {
            switch ((SDL.EventType)@event.Type)
            {
                case SDL.EventType.Quit:
                    ProcessQuitEvent(false);
                    break;
            }
            EventProcess?.Invoke(in @event);
        }
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
    
    private void ThrowIfNativeHandleInvalid()
    {
        if (NativeHandle == IntPtr.Zero)
            throw new NativeWindowException("Window is not yet initialized or has been disposed");
    }
}
