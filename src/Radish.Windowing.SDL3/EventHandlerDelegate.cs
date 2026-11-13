namespace Radish.Windowing.SDL3;

/// <summary>
/// Event invoked for every <see cref="SDL_Event"/> processed by <see cref="SDL_PollEvent(out SDL_Event)"/>.
/// <param name="event">The event data structure.</param>
/// </summary>
public delegate void EventHandlerDelegate(in SDL_Event @event);
