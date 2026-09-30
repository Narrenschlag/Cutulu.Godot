#if GODOT4_0_OR_GREATER
namespace Cutulu.Core;

using System;
using System.Threading.Tasks;
using Godot;

/// <summary>
/// Provides easy to access screenshot functionality.
/// </summary>
public static class Screenshot
{
    /// <summary>
    /// Returns an image of the current view of the camera. Uses current window size as resolution.
    /// This does not include overlays or any kind of user-interface not visible to the camera itself.
    /// </summary>
    public static async Task<Image> Create(this Camera3D camera)
    => await Create(camera, Settings.GraphicSettings.WindowSize);

    /// <summary>
    /// Returns an image of the current view of the camera. 
    /// This does not include overlays or any kind of user-interface not visible to the camera itself.
    /// </summary>
    public static async Task<Image> Create(
        this Camera3D camera,
        Vector2I resolution)
    {
        resolution = new Vector2I(Mathf.Abs(resolution.X), Mathf.Abs(resolution.Y));
        var transform = camera.GlobalTransform;
        var parent = camera;

        var viewport = new SubViewport()
        {
            Size = resolution,
            RenderTargetUpdateMode = SubViewport.UpdateMode.Once,
        };
        parent.AddChild(viewport);

        camera = camera.Duplicate() as Camera3D;
        viewport.AddChild(camera);

        camera.GlobalTransform = transform;

        try
        {
            // Wait for the viewport to finish rendering.
            await viewport.ToSignal(
                RenderingServer.Singleton,
                RenderingServer.SignalName.FramePostDraw
            );
        }
        finally
        {
            camera.Destroy();
            viewport.Destroy();
        }

        // Retrieve the rendered image.
        return viewport.GetTexture().GetImage();
    }
}
#endif