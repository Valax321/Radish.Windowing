# Radish.Windowing

A simple little .NET window management system modelled off Silk.NET.Windowing but without its various common/math/rendering dependencies. It also has extensive input device APIs and fully tested NativeAOT support.

It uses SDL3 as its main (and currently only) window backend with the [SDL3-CS](https://github.com/flibitijibibo/SDL3-CS) bindings. This backend is designed to primarily work with SDL_GPU as a rendering API. The SDL3 provider NuGet package ships with prebuilt SDL3 native Windows binaries for x86, x64 and arm64 (these DLLs are sourced directly from official SDL3 binary releases).

## Upcoming Features

* A touchscreen input device still needs to be implemented.
* Look more into SDL's gamepad haptics APIs to see if PS5 trigger stuff can be implemented.
* More generally, we still need to expose some gamepad features like the PS4 lightbar and rumble.
* Work out a way to have a proper test suite for this kind of library.

## License

This project uses the zlib license. See [LICENSE](./LICENSE) for the full text. Versions prior to 1.0.0 were licensed under MIT, although that is no longer the case.
