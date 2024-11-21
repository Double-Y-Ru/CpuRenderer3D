using System.Numerics;

namespace CpuRenderer3D
{
    public class Engine
    {
        public void Render(SceneNode scene, Camera camera, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
        {
            RenderingContext renderingContext = new RenderingContext(
                colorBuffer,
                depthBuffer,
                worldView: camera.GetWorldViewMatrix(),
                viewProjection: camera.GetViewProjectionMatrix(),
                projectionClip: CreateProjectionClipMatrix(colorBuffer.Width, colorBuffer.Height));

            RenderRecursive(scene, renderingContext);
        }

        private static void RenderRecursive(SceneNode sceneNode, RenderingContext renderingContext)
        {
            renderingContext.SetModelWorld(sceneNode.GlobalTransform.GetMatrix());

            foreach (IRenderer renderer in sceneNode.GetRenderers())
                renderer.Render(renderingContext);

            foreach (SceneNode child in sceneNode.GetChildren())
                RenderRecursive(child, renderingContext);
        }

        private static Matrix4x4 CreateProjectionClipMatrix(int bufferWidth, int bufferHeight)
        {
            return new Matrix4x4(
                0.5f * bufferWidth, 0, 0, 0,
                0, 0.5f * bufferHeight, 0, 0,
                0, 0, 1, 0,
                0.5f * bufferWidth, 0.5f * bufferHeight, 0, 1);
        }
    }
}