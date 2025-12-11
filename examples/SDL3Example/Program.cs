using Radish.Windowing;
using Radish.Windowing.SDL3;
using SDL3;
using SDL3Example;

// These are supposed to be set before SDL_Init (which registering the provider eventually calls into)
SDL.SetAppMetadata("SDL3 Windowing Example", "1.0.0", "com.radish.sdl3-example");
SDL.SetAppMetadataProperty(SDL.Props.AppMetadataTypeString, "game");
SDL.SetAppMetadataProperty(SDL.Props.AppMetadataCreatorString, "Radish Games");

// Register the window provider (this step is vital or else the library will not know about what windowing backends
// are available).
WindowingProvider.RegisterProvider<Sdl3WindowingProvider>();

// Create an application instance and run it
using var app = new Application();
app.Run();
