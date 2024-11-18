using System.Numerics;

namespace CpuRenderer3D
{
    public class MeshRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram _shaderProgram;

        private readonly FragmentInput[] _fragVerticesCache;

        public MeshRenderer(Mesh mesh, IShaderProgram shaderProgram)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _fragVerticesCache = new FragmentInput[_mesh.GetVertices().Length];
        }

        public void Render(RenderingContext renderingContext)
        {
            for (int vid = 0; vid < _mesh.GetVertices().Length; ++vid)
            {
                VertexInput vertInput = new VertexInput(_mesh.GetVertex(vid), new Vector3(), new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                _fragVerticesCache[vid] = _shaderProgram.ComputeVertex(vertInput, renderingContext);
            }

            for (int i = 0; i < _mesh.GetTriangles().Length; i++)
            {
                Triangle triangle = _mesh.GetTriangles()[i];

                FragmentInput fragInput0 = _fragVerticesCache[triangle.V0];
                FragmentInput fragInput1 = _fragVerticesCache[triangle.V1];
                FragmentInput fragInput2 = _fragVerticesCache[triangle.V2];

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