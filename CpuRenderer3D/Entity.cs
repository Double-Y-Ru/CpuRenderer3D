namespace CpuRenderer3D
{
    public class Entity
    {
        public Transform Transform;
        public Mesh Mesh;
        public IRenderer MeshRenderer;
        public IShaderProgram ShaderProgram;

        public Entity(Transform transform, Mesh mesh, IShaderProgram shaderProgram)
        {
            Transform = transform;
            Mesh = mesh;
            ShaderProgram = shaderProgram;
            MeshRenderer = new MeshRenderer();
        }
    }
}
