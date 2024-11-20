using System.Numerics;

namespace CpuRenderer3D
{
    public class MeshRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram _shaderProgram;

        public MeshRenderer(Mesh mesh, IShaderProgram shaderProgram)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
        }

        public void Render(RenderingContext renderingContext)
        {
            for (int i = 0; i < _mesh.GetTriangles().Length; i++)
            {
                Triangle triangle = _mesh.GetTriangles()[i];

                VertexInput vertInput0 = new VertexInput(_mesh.GetVertex(triangle.Vertex0.VertexIndex), _mesh.GetNormal(triangle.Vertex0.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex0.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput1 = new VertexInput(_mesh.GetVertex(triangle.Vertex1.VertexIndex), _mesh.GetNormal(triangle.Vertex1.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex1.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput2 = new VertexInput(_mesh.GetVertex(triangle.Vertex2.VertexIndex), _mesh.GetNormal(triangle.Vertex2.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex2.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());

                FragmentInput fragInput0 = _shaderProgram.ComputeVertex(vertInput0, renderingContext);
                FragmentInput fragInput1 = _shaderProgram.ComputeVertex(vertInput1, renderingContext);
                FragmentInput fragInput2 = _shaderProgram.ComputeVertex(vertInput2, renderingContext);

                Vector3 triangleNormalP = Vector3.Cross(
                    fragInput0.Position - fragInput1.Position,
                    fragInput0.Position - fragInput2.Position);

                if (Vector3.Dot(triangleNormalP, Vector3.UnitZ) < 0) continue;

                if (-1f < fragInput0.Position.X && fragInput0.Position.X < 1f
                 && -1f < fragInput0.Position.Y && fragInput0.Position.Y < 1f
                 && -1f < fragInput0.Position.Z && fragInput0.Position.Z < 1f
                 && -1f < fragInput1.Position.X && fragInput1.Position.X < 1f
                 && -1f < fragInput1.Position.Y && fragInput1.Position.Y < 1f
                 && -1f < fragInput1.Position.Z && fragInput1.Position.Z < 1f
                 && -1f < fragInput2.Position.X && fragInput2.Position.X < 1f
                 && -1f < fragInput2.Position.Y && fragInput2.Position.Y < 1f
                 && -1f < fragInput2.Position.Z && fragInput2.Position.Z < 1f
                 )
                {
                    fragInput0.Position = Vector3.Transform(fragInput0.Position, renderingContext.ProjectionClip);
                    fragInput1.Position = Vector3.Transform(fragInput1.Position, renderingContext.ProjectionClip);
                    fragInput2.Position = Vector3.Transform(fragInput2.Position, renderingContext.ProjectionClip);

                    Drawer.DrawTriangle(renderingContext, _shaderProgram, fragInput0, fragInput1, fragInput2);
                }
            }
        }
    }
}