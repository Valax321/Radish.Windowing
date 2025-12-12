using System.Drawing;
using SDL3;

namespace Radish.Windowing.SDL3;

internal sealed class SdlDisplay(uint displayId) : IDisplay
{
    public uint DisplayIndex => displayId;

    public VideoMode NativeVideoMode
    {
        get
        {
            var dm = SDL.GetDesktopDisplayMode(displayId);
            if (!dm.HasValue)
                throw new NativeWindowException(SDL.GetError());
            return new VideoMode(new Size(dm.Value.W, dm.Value.H), dm.Value.RefreshRate);
        }
    }

    public VideoMode CurrentVideoMode
    {
        get
        {
            var dm = SDL.GetCurrentDisplayMode(displayId);
            if (!dm.HasValue)
                throw new NativeWindowException(SDL.GetError());
            return new VideoMode(new Size(dm.Value.W, dm.Value.H), dm.Value.RefreshRate);
        }
    }

    public IEnumerable<VideoMode> FullscreenVideoModes
    {
        get
        {
            var modes = SDL.GetFullscreenDisplayModes(displayId, out _);
            if (modes == null)
                throw new NativeWindowException(SDL.GetError());

            foreach (var dm in modes)
                yield return new VideoMode(new Size(dm.W, dm.H), dm.RefreshRate);
        }
    }

    public float ContentScale => SDL.GetDisplayContentScale(displayId);

    public bool SupportsHDR
    {
        get
        {
            var props = SDL.GetDisplayProperties(displayId);
            if (props == 0)
                throw new NativeWindowException(SDL.GetError());
            
            return SDL.GetBooleanProperty(props, SDL.Props.DisplayHDREnabledBoolean, false);
        }
    }

    public IntPtr NativeHandle => (IntPtr)displayId;

    public override string ToString()
    {
        var n = SDL.GetDisplayName(displayId);
        if (n == null)
            throw new NativeWindowException(SDL.GetError());
        return n;
    }

    private bool Equals(SdlDisplay other)
    {
        return DisplayIndex == other.DisplayIndex;
    }

    public bool Equals(IDisplay? other)
    {
        return other is SdlDisplay sd && Equals(sd);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is SdlDisplay other && Equals(other);
    }

    public override int GetHashCode()
    {
        return DisplayIndex.GetHashCode();
    }

    public static bool operator ==(SdlDisplay? left, SdlDisplay? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SdlDisplay? left, SdlDisplay? right)
    {
        return !Equals(left, right);
    }
}