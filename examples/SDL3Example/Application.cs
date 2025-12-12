using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using ColorHelper;
using Radish.Windowing.InputDevices;
using Radish.Windowing.SDL3;
using SDL3;
using ColorConverter = ColorHelper.ColorConverter;

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
    private Vector2 _squarePos = new(1280 / 2f, 720 / 2f);
    private float _hue;
    
    public Application()
    {
        // Create a window instance.
        _window = WindowingProvider.CreateWindow(new WindowInitParameters
        {
            Size = new Size(1280, 720),
            Title = "SDL3 Windowing Example",
            Resizable = true,
            MinimumSize = new Size(640, 480),
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

        if (_input.PrimaryMouse != null)
        {
            _squarePos += _input.PrimaryMouse.PositionDelta;
            _hue += _input.PrimaryMouse.WheelAxes.Y;
        }
    }

    private void OnWindowRender(TimeSpan deltaTime)
    {
        var s = _window.ContentScale;
        SDL.SetRenderScale(_graphicsDevice, s, s);
        
        SDL.SetRenderDrawColor(_graphicsDevice, 
            Color.DarkSlateGray.R, Color.DarkSlateGray.G, 
            Color.DarkSlateGray.B, Color.DarkSlateGray.A);
        SDL.RenderClear(_graphicsDevice);
        
        // Draw a square to visualise relative mouse movement
        var c = ColorConverter.HsvToRgb(new HSV(
            (int)_hue,
            100, 100)
        );
        SDL.SetRenderDrawColor(_graphicsDevice, c.R, c.G, c.B, (byte)SDL.AlphaOpaque);
        SDL.RenderFillRect(_graphicsDevice, new SDL.FRect
        {
            X = _squarePos.X + 8,
            Y = _squarePos.Y + 8,
            W = 16,
            H = 16
        });

        SDL.SetRenderDrawColor(_graphicsDevice, 255, 255, 255, (byte)SDL.AlphaOpaque);

        var dp = new Point(12, 12);
        
        PrintSomeTextPlease("Radish.Windowing Debugger Program for SDL3");
        PrintSomeTextPlease($"Runtime: {RuntimeInformation.FrameworkDescription}");
        PrintSomeTextPlease($"OS: {RuntimeInformation.OSDescription}");
        PrintSomeTextPlease($"SDL version: {VersionUtility.ParseSdlVersion(SDL.GetVersion())}");
        
        dp.Y += 12;
        PrintSomeTextPlease("-----------------------------------------------");
        
        dp.Y += 12;
        PrintSomeTextPlease($"Input Devices: {_input!.InputDevices.Count}");
        foreach (var device in _input.InputDevices)
        {
            PrintSomeTextPlease($"  Device: {device} ({device.GetType().Name}), DeviceId: {device.NativeHandle}");
        }
        
        dp.Y += 12;
        PrintSomeTextPlease($"Gamepads: {_input.Gamepads.Count}");
        foreach (var gp in _input.Gamepads)
        {
            PrintSomeTextPlease($"  {gp} ({gp.Model}): {MakeBitStringFromGamepadStates(gp)}");
            for (var i = 0; i < gp.Touchpads.Count; ++i)
            {
                PrintSomeTextPlease($"    Touchpad {i + 1}: {string.Join(", ", gp.Touchpads[i].Fingers.Select(x => $"{x.Position:F3} {x.Pressure:F3}"))}");
            }
        }
        
        dp.Y += 12;
        PrintSomeTextPlease($"Text Input Buffer (Editing: {_input.TextInputActive}): {_textInputBuffer}|");
        PrintSomeTextPlease("Press ~ to begin editing...");
        
        dp.Y += 12;
        PrintSomeTextPlease($"Keyboards: {_input.Keyboards.Count}");
        foreach (var kb in _input.Keyboards)
        {
            PrintSomeTextPlease($"  {kb}: {MakeBitStringFromKeyboardStates(kb)}");
        }
        
        dp.Y += 12;
        PrintSomeTextPlease($"Mice: {_input.Mice.Count}");
        foreach (var m in _input.Mice)
        {
            PrintSomeTextPlease($"  {m}: {MakeBitStringFromMouseStates(m)}");
        }
        
        dp.Y += 12;
        PrintSomeTextPlease("-----------------------------------------------");
        
        dp.Y += 12;
        var dps = _window.Displays.ToArray();
        PrintSomeTextPlease($"Window Pos: {_window.Position}, Size: {_window.Size}, PixelSize: {_window.PixelSize}, Mode: {_window.FullscreenMode}, Focused: {_window.Focused}, Scale: {_window.ContentScale}");
        PrintSomeTextPlease($"Displays: {dps.Length}");
        var cd = _window.CurrentDisplay;
        foreach (var d in dps)
        {
            var v = d.CurrentVideoMode;
            PrintSomeTextPlease($"  {d} {(d.Equals(cd) ? "(Current)" : string.Empty)}: {v.Resolution} @ {v.RefreshRate:F2}Hz, HDR: {d.SupportsHDR}, Content Scale: {d.ContentScale}");
            PrintSomeTextPlease($"    Supports {d.FullscreenVideoModes.Count()} video modes");
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
            if (k == GamepadButtons.None || !gamepad.IsPressed(k))
                continue;

            sb.Append($"{k} ");
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
            if (k == Scancodes.None || !keyboard.IsPressed(k))
                continue;

            sb.Append($"{k} ");
        }
        return sb.ToString();
    }
    
    private static string MakeBitStringFromMouseStates(IMouse mouse)
    {
        var sb = new StringBuilder();
        foreach (var k in Enum.GetValues<MouseButtons>())
        {
            if (k == MouseButtons.None || !mouse.IsPressed(k))
                continue;

            sb.Append($"{k} ");
        }

        sb.Append($" {mouse.Position:F1}");
        sb.Append($" {mouse.PositionDelta:F4}");
        sb.Append($" {mouse.WheelAxes:F2}");
        
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
            _input!.BeginTextInput();
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