using Radish.Windowing.InputDevices;

namespace Radish.Windowing.SDL3.InputDevices;

internal abstract class SdlBaseInputDevice : IInputDevice, 
    IComparable<SdlBaseInputDevice>, 
    IEquatable<SdlBaseInputDevice>
{
    public IntPtr NativeHandle { get; private init; }

    public uint InstanceId
    {
        get => (uint)NativeHandle;
        protected init => NativeHandle = (IntPtr)value;
    }

    public abstract void ClearEvents();

    public int CompareTo(SdlBaseInputDevice? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (other is null) return 1;
        return NativeHandle.CompareTo(other.NativeHandle);
    }

    public bool Equals(SdlBaseInputDevice? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return NativeHandle == other.NativeHandle;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((SdlBaseInputDevice)obj);
    }

    public override int GetHashCode()
    {
        return NativeHandle.GetHashCode();
    }
}