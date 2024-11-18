namespace CpuRenderer3D
{
    public class Entity
    {
        public Transform Transform;
        public Mesh Mesh;
        public IRenderer MeshRenderer;
        public IRenderer EdgeRenderer;

        public Entity(Transform transform, Mesh mesh, IShaderProgram meshShaderProgram, IShaderProgram edgeShaderProgram)
        {
            Transform = transform;
            Mesh = mesh;
            MeshRenderer = new MeshRenderer(mesh, meshShaderProgram);
            EdgeRenderer = new ContourRenderer(mesh, edgeShaderProgram);
        }
    }
}
