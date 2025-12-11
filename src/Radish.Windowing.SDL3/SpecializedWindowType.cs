using JetBrains.Annotations;

namespace Radish.Windowing.SDL3;

/// <summary>
/// Options for creating a specialised type of window, as required by some graphics APIs.
/// Note: if using SDL_GPU specifying these flags is not required.
/// </summary>
[PublicAPI]
public enum SpecializedWindowType
{
    /// <summary>
    /// No special type.
    /// </summary>
    None,
    /// <summary>
    /// Create an OpenGL-compatible window. Without this using OpenGL with this window may fail.
    /// </summary>
    OpenGLCompatible,
    /// <summary>
    /// Create a Vulkan-compatible window. Without this using Vulkan with this window may fail.
    /// </summary>
    VulkanCompatible,
    /// <summary>
    /// Create a Metal-compatible window. Without this using Metal with this window may fail.
    /// </summary>
    MetalCompatible,
}