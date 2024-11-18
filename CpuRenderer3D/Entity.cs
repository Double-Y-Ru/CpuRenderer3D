namespace CpuRenderer3D
{
    public class Entity
    {
        public Transform Transform;
        public Mesh Mesh;
        public IRenderer MeshRenderer;
        public IShaderProgram MeshShaderProgram;

        public IRenderer EdgeRenderer;
        public IShaderProgram EdgeShaderProgram;

        public Entity(Transform transform, Mesh mesh, IShaderProgram meshShaderProgram, IShaderProgram edgeShaderProgram)
        {
            Transform = transform;
            Mesh = mesh;
            MeshShaderProgram = meshShaderProgram;
            EdgeShaderProgram = edgeShaderProgram;
            MeshRenderer = new MeshRenderer();
            EdgeRenderer = new EdgeRenderer();
        }
    }
}
