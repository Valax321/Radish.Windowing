# Radish.Windowing

A simple little .NET window management system modelled off Silk.NET.Windowing but without its various common/math/rendering dependencies, some extended APIs and fully tested NativeAOT support.

It uses SDL3 as its main (and currently only) window backend with the [SDL3-CS](https://github.com/edwardgushchin/SDL3-CS) bindings. This backend is designed to primarily work with SDL_GPU as a rendering API.

## License

This project uses the MIT license. See [LICENSE](./LICENSE) for the full text.
