using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
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
    private readonly StringBuilder _textInputBuffer = new();
    
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
        _input.TextInput += OnTextInput;

        _graphicsDevice = SDL.CreateRenderer(_window.NativeHandle, null);
        if (_graphicsDevice == IntPtr.Zero)
            throw new NativeWindowException(SDL.GetError());

        SDL.SetRenderLogicalPresentation(_graphicsDevice, 1280, 720, 
            SDL.RendererLogicalPresentation.IntegerScale);
    }

    private void OnTextInput(ReadOnlySpan<char> text)
    {
        _textInputBuffer.Append(text);
    }

    private void OnWindowUpdate(TimeSpan deltaTime)
    {
        var kb = _input!.Keyboards.FirstOrDefault();
        if (kb != null && kb.IsPressed(kb.KeycodeToScancode(Keys.Escape)))
            _window.Close();
    }

    private void OnWindowRender(TimeSpan deltaTime)
    {
        SDL.SetRenderDrawColor(_graphicsDevice, 
            Color.CornflowerBlue.R, Color.CornflowerBlue.G, 
            Color.CornflowerBlue.B, Color.CornflowerBlue.A);
        SDL.RenderClear(_graphicsDevice);

        SDL.SetRenderDrawColor(_graphicsDevice, 255, 255, 255, (byte)SDL.AlphaOpaque);

        var dp = new Point(12, 12);
        
        PrintSomeTextPlease($"Devices: {_input!.InputDevices.Count}");
        foreach (var device in _input.InputDevices)
        {
            PrintSomeTextPlease($"  Device: {device} ({device.GetType().Name}), DeviceId: {device.NativeHandle}");
        }
        
        dp.Y += 24;
        PrintSomeTextPlease($"Gamepads: {_input.Gamepads.Count}");
        foreach (var gp in _input.Gamepads)
        {
            PrintSomeTextPlease($"  {gp} ({gp.Model}): {MakeBitStringFromGamepadStates(gp)}");
            for (var i = 0; i < gp.Touchpads.Count; ++i)
            {
                PrintSomeTextPlease($"    Touchpad {i + 1}: {string.Join(", ", gp.Touchpads[i].Fingers.Select(x => $"{x.Position:F3} {x.Pressure:F3}"))}");
            }
        }
        
        dp.Y += 24;
        PrintSomeTextPlease($"Text Input Buffer (Editing: {_input.TextInputActive}): {_textInputBuffer}|");
        PrintSomeTextPlease($"Keyboards: {_input.Keyboards.Count}");
        foreach (var kb in _input.Keyboards)
        {
            PrintSomeTextPlease($"  {kb}: {MakeBitStringFromKeyboardStates(kb)}");
        }
        
        dp.Y += 24;
        PrintSomeTextPlease($"Mice: {_input.Mice.Count}");
        foreach (var m in _input.Mice)
        {
            PrintSomeTextPlease($"  {m}: {MakeBitStringFromMouseStates(m)}");
        }
        
        SDL.RenderPresent(_graphicsDevice);
        return;

        void PrintSomeTextPlease(string text)
        {
            SDL.RenderDebugText(_graphicsDevice, dp.X, dp.Y, text);
            dp.Y += 12;
        }
    }

    private static string MakeBitStringFromGamepadStates(IGamepad gamepad)
    {
        var sb = new StringBuilder();
        foreach (var k in Enum.GetValues<GamepadButtons>())
        {
            if (k == GamepadButtons.None)
                continue;

            sb.Append(gamepad.IsPressed(k) ? '1' : '0');
        }
        
        foreach (var a in Enum.GetValues<GamepadAxes>())
        {
            if (a == GamepadAxes.None)
                continue;

            sb.Append($" {gamepad.GetAxisValue(a):F3}");
        }
        
        return sb.ToString();
    }
    
    private static string MakeBitStringFromKeyboardStates(IKeyboard keyboard)
    {
        var sb = new StringBuilder();
        foreach (var k in Enum.GetValues<Scancodes>())
        {
            if (k == Scancodes.None)
                continue;

            sb.Append(keyboard.IsPressed(k) ? '1' : '0');
        }
        return sb.ToString();
    }
    
    private static string MakeBitStringFromMouseStates(IMouse mouse)
    {
        var sb = new StringBuilder();
        foreach (var k in Enum.GetValues<MouseButtons>())
        {
            if (k == MouseButtons.None)
                continue;

            sb.Append(mouse.IsPressed(k) ? '1' : '0');
        }

        sb.Append($" {mouse.Position}");
        sb.Append($" {mouse.PositionDelta}");
        sb.Append($" {mouse.WheelAxes}");
        
        return sb.ToString();
    }

    private void OnWindowClosing()
    {
        Console.WriteLine("Window closing");
        
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
    
    private void OnDeviceAdded(IInputDevice device)
    {
        Console.WriteLine($"{device.GetType().Name} added: {device}");

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
        Console.WriteLine($"Device removed: {device}");
    }
    
    private void MouseButtonUp(MouseButtons button, bool isDown)
    {
    }

    private void KbButtonUp(Keys key, Scancodes scancode, bool isDown)
    {
        if (key == Keys.Grave && isDown && !_input!.TextInputActive)
        {
            _input!.BeginTextInput(TextInputType.Number);
        }

        if (key == Keys.Return && isDown && _input!.TextInputActive)
        {
            _input.EndTextInput();
            _textInputBuffer.Clear();
        }

        if (key == Keys.Backspace && isDown && _input!.TextInputActive)
        {
            if (_textInputBuffer.Length > 0)
                _textInputBuffer.Remove(_textInputBuffer.Length - 1, 1);
        }
    }

    private void GpButtonUp(GamepadButtons button, bool isDown)
    {
    }
}