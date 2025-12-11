using Radish.Windowing.InputDevices;

namespace Radish.Windowing.SDL3.InputDevices;

internal abstract class SdlBaseInputDevice : IInputDevice
{
    public IntPtr NativeHandle { get; private init; }

    public uint InstanceId
    {
        get => (uint)NativeHandle;
        protected init => NativeHandle = (IntPtr)value;
    }
}