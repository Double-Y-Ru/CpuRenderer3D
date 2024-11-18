using System.Numerics;

namespace CpuRenderer3D
{
    public class CpuRenderer
    {
        public void Render(IReadOnlyList<Entity> entities, Camera camera, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
        {
            RenderingContext renderingContext = new RenderingContext(
                colorBuffer,
                depthBuffer,
                worldView: camera.GetWorldViewMatrix(),
                viewProjection: camera.GetViewProjectionMatrix(),
                projectionClip: Util.CreateProjectionClip(colorBuffer.Width, colorBuffer.Height));

            foreach (Entity entity in entities)
            {
                entity.MeshRenderer.Render(entity, renderingContext, entity.MeshShaderProgram);
                entity.EdgeRenderer.Render(entity, renderingContext, entity.EdgeShaderProgram);
            }
        }
    }
}