using System.Drawing;
using Radish.Windowing;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3;

namespace SDL3Example;

/// <summary>
/// Manages the callbacks into the window class and disposing of it.
/// </summary>
public class Application : IDisposable
{
    private readonly IWindow _window;
    private IInputContext? _input;
    
    public Application()
    {
        // Create a window instance.
        _window = WindowingProvider.CreateWindow(new WindowInitParameters
        {
            Size = new Size(1280, 720),
            Title = "SDL3 Windowing Example",
            // ReSharper disable once HeapView.BoxingAllocation
            BackendParameters = new Sdl3BackendWindowParameters
            {
                // We aren't using any special behaviour here for this example
                // but for the sake of demonstrating as much of the API area as
                // possible we are manually specifying these.
                
                WindowType = SpecializedWindowType.None
            }
        });

        // Hook into the window lifecycle events
        _window.Loaded += OnWindowLoad;
        _window.Update += OnWindowUpdate;
        _window.Render += OnWindowRender;
        _window.Closing += OnWindowClosing;
    }

    public void Run() 
        => _window.Run();
    
    private void OnWindowLoad()
    {
        // In here the window is fully set up and valid to work on.
        // All post-construction initialisation should be done here.
        _input = _window.CreateInput();
    }

    private void OnWindowUpdate(TimeSpan deltaTime)
    {
        // This will never happen in this example, but it makes the nullability warnings shut up.
        ArgumentNullException.ThrowIfNull(_input);
    }

    private void OnWindowRender(TimeSpan deltaTime)
    {
        // This will never happen in this example, but it makes the nullability warnings shut up.
        ArgumentNullException.ThrowIfNull(_input);
    }

    private void OnWindowClosing()
    {
        
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _input?.Dispose();
        _window.Dispose();
    }
}