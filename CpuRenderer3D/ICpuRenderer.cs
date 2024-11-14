namespace CpuRenderer3D
{
    public interface ICpuRenderer
    {
        void Render(Transform camera, Entity[] entities, Bytemap bytemap);
        void Render(Entity[] entities, ShaderProgram shaderProgram, RenderingContext renderingContext);
    }
}