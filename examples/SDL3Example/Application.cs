using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3;
using SDL3;

namespace Radish.Windowing.Examples;

/// <summary>
/// Manages the callbacks into the window class and disposing of it.
/// </summary>
public class Application : IDisposable
{
    private readonly IWindow _window;
    private IInputContext? _input;
    private IntPtr _graphicsDevice = IntPtr.Zero;
    
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
    
    [SuppressMessage("ReSharper", "BitwiseOperatorOnEnumWithoutFlags")]
    private void OnWindowLoad()
    {
        Console.WriteLine("Window loaded");
        
        // In here the window is fully set up and valid to work on.
        // All post-construction initialisation should be done here.
        _input = _window.CreateInput();
        _input.DeviceAdded += OnDeviceAdded;
        _input.DeviceRemoved += OnDeviceRemoved;

        SDL.GPUShaderFormat formats = 0;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            formats |= SDL.GPUShaderFormat.DXBC;
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            formats |= SDL.GPUShaderFormat.MSL;
        }
        else
        {
            formats |= SDL.GPUShaderFormat.SPIRV;
        }
        
        _graphicsDevice = SDL.CreateGPUDevice(formats, true, null);
        if (_graphicsDevice == IntPtr.Zero)
            throw new NativeWindowException(SDL.GetError());

        if (!SDL.ClaimWindowForGPUDevice(_graphicsDevice, _window.NativeHandle))
            throw new NativeWindowException(SDL.GetError());
    }

    private void OnDeviceAdded(IInputDevice device)
    {
        Console.WriteLine($"{device.GetType().Name} added: {device} (0x{device.NativeHandle:x4})");

        switch (device)
        {
            case IGamepad gp:
                Console.WriteLine($" Type: {gp.Model}, Player index: {gp.PlayerIndex}, Touchpad Count: {gp.Touchpads.Count}");
                gp.ButtonDown += GpButtonUp;
                break;
            case IKeyboard kb:
                kb.KeyDown += KbButtonUp;
                break;
            case IMouse m:
                m.ButtonDown += MouseButtonUp;
                break;
        }
    }

    private void OnDeviceRemoved(IInputDevice device)
    {
        Console.WriteLine($"Device removed: {device} (0x{device.NativeHandle:x4})");
    }
    
    private void MouseButtonUp(MouseButtons button, bool isDown)
    {
        Console.WriteLine($"Pressed: {button} {isDown}");
    }

    private void KbButtonUp(Keys key, Scancodes scancode, bool isDown)
    {
        Console.WriteLine($"Pressed: {key} {isDown}");
    }

    private void GpButtonUp(GamepadButtons button, bool isDown)
    {
        Console.WriteLine($"Pressed: {button} {isDown}");
    }

    private void OnWindowUpdate(TimeSpan deltaTime)
    {
        var kb = _input!.Keyboards.FirstOrDefault();
        if (kb != null && kb.IsPressed(kb.KeycodeToScancode(Keys.Escape)))
            _window.Close();
    }

    private unsafe void OnWindowRender(TimeSpan deltaTime)
    {
        var commandBuffer = SDL.AcquireGPUCommandBuffer(_graphicsDevice);
        if (!SDL.WaitAndAcquireGPUSwapchainTexture(commandBuffer, _window.NativeHandle, out var swapchainTex,
                out _,
                out _))
        {
            Console.Error.WriteLine(SDL.GetError());
            return;
        }

        if (swapchainTex != IntPtr.Zero)
        {
            var targetInfo = new SDL.GPUColorTargetInfo()
            {
                Texture = swapchainTex,
                ClearColor = Color.CornflowerBlue.ToSdlColor(),
                LoadOp = SDL.GPULoadOp.Clear,
                StoreOp = SDL.GPUStoreOp.Store
            };

            var pass = SDL.BeginGPURenderPass(commandBuffer, (IntPtr)(&targetInfo), 
                1, IntPtr.Zero);
            SDL.EndGPURenderPass(pass);
        }

        SDL.SubmitGPUCommandBuffer(commandBuffer);
    }

    private void OnWindowClosing()
    {
        Console.WriteLine("Window closing");
        
        SDL.ReleaseWindowFromGPUDevice(_graphicsDevice, _window.NativeHandle);
        
        if (_graphicsDevice != IntPtr.Zero)
        {
            SDL.DestroyRenderer(_graphicsDevice);
            _graphicsDevice = IntPtr.Zero;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _input?.Dispose();
        _window.Dispose();
    }
}