namespace CpuRenderer3D
{
    public class CpuRenderer
    {
        public void Render(IReadOnlyList<Entity> entities, IShaderProgram shaderProgram, RenderingContext renderingContext)
        {
            foreach (Entity entity in entities)
                entity.Render(renderingContext, shaderProgram);
        }
    }
}