using System.Numerics;

namespace CpuRenderer3D
{
    public class CpuRenderer
    {
        public void Render(SceneNode scene, Camera camera, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
        {
            RenderingContext renderingContext = new RenderingContext(
                colorBuffer,
                depthBuffer,
                worldView: camera.GetWorldViewMatrix(),
                viewProjection: camera.GetViewProjectionMatrix(),
                projectionClip: Util.CreateProjectionClip(colorBuffer.Width, colorBuffer.Height));

            RenderRecursive(scene, renderingContext);
        }

        public void RenderRecursive(SceneNode sceneNode, RenderingContext renderingContext)
        {
            renderingContext.SetModelWorld(sceneNode.GlobalTransform.GetMatrix());

            foreach (IRenderer renderer in sceneNode.GetRenderers())
                renderer.Render(renderingContext);

            foreach (SceneNode child in sceneNode.GetChildren())
                RenderRecursive(child, renderingContext);
        }
    }
}