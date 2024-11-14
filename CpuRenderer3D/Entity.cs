namespace CpuRenderer3D
{
    public class Entity
    {
        public Transform Transform;
        public Mesh Mesh;

        public IRenderer MeshRenderer;

        public Entity(Transform transform, Mesh mesh)
        {
            Transform = transform;
            Mesh = mesh;
            MeshRenderer = new MeshRenderer();
        }

        public void Render(RenderingContext renderingContext, ShaderProgram shaderProgram)
        {
            MeshRenderer.Render(this, renderingContext, shaderProgram);
        }
    }
}
