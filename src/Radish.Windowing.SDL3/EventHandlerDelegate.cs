using SDL3;

namespace Radish.Windowing.SDL3;

/// <summary>
/// Event invoked for every <see cref="SDL.Event"/> processed by <see cref="SDL.PollEvent"/>.
/// <param name="event">The event data structure.</param>
/// </summary>
public delegate void EventHandlerDelegate(in SDL.Event @event);
