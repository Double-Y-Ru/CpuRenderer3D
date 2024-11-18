using System.Numerics;

namespace CpuRenderer3D
{
    public class ContourRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram _shaderProgram;

        private readonly FragmentInput[] _fragVerticesCache;

        public ContourRenderer(Mesh mesh, IShaderProgram shaderProgram)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _fragVerticesCache = new FragmentInput[_mesh.GetVertices().Length];
        }

        public void Render(RenderingContext renderingContext)
        {
            for (int vid = 0; vid < _mesh.GetVertices().Length; ++vid)
            {
                VertexInput vertInput = new VertexInput(_mesh.GetVertex(vid), new Vector3(), new Vector4(0.8f, 0.8f, 0.8f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                _fragVerticesCache[vid] = _shaderProgram.ComputeVertex(vertInput, renderingContext);
            }

            for (int eid = 0; eid < _mesh.GetEdges().Length; eid++)
            {
                Edge edge = _mesh.GetEdges()[eid];

                FragmentInput fragInput0 = _fragVerticesCache[edge.V0];
                FragmentInput fragInput1 = _fragVerticesCache[edge.V1];

                if (edge.Tris.Length == 2)
                {
                    Triangle triangle0 = _mesh.GetTriangles()[edge.Tris[0]];
                    Triangle triangle1 = _mesh.GetTriangles()[edge.Tris[1]];

                    FragmentInput triangle0Vert0 = _fragVerticesCache[triangle0.V0];
                    FragmentInput triangle0Vert1 = _fragVerticesCache[triangle0.V1];
                    FragmentInput triangle0Vert2 = _fragVerticesCache[triangle0.V2];

                    FragmentInput triangle1Vert0 = _fragVerticesCache[triangle1.V0];
                    FragmentInput triangle1Vert1 = _fragVerticesCache[triangle1.V1];
                    FragmentInput triangle1Vert2 = _fragVerticesCache[triangle1.V2];

                    Vector3 triangle0NormalP = Vector3.Cross(
                        triangle0Vert0.Position - triangle0Vert1.Position,
                        triangle0Vert0.Position - triangle0Vert2.Position);

                    Vector3 triangle1NormalP = Vector3.Cross(
                        triangle1Vert0.Position - triangle1Vert1.Position,
                        triangle1Vert0.Position - triangle1Vert2.Position);

                    bool triangle0IsCameraFaced = Vector3.Dot(triangle0NormalP, Vector3.UnitZ) > 0;
                    bool triangle1IsCameraFaced = Vector3.Dot(triangle1NormalP, Vector3.UnitZ) > 0;

                    if (!triangle0IsCameraFaced && !triangle1IsCameraFaced) continue;
                    if (triangle0IsCameraFaced == triangle1IsCameraFaced) continue;
                }

                if (-1f < fragInput0.Position.X && fragInput0.Position.X < 1f
                 && -1f < fragInput0.Position.Y && fragInput0.Position.Y < 1f
                 && -1f < fragInput0.Position.Z && fragInput0.Position.Z < 1f
                 && -1f < fragInput1.Position.X && fragInput1.Position.X < 1f
                 && -1f < fragInput1.Position.Y && fragInput1.Position.Y < 1f
                 && -1f < fragInput1.Position.Z && fragInput1.Position.Z < 1f)
                {
                    fragInput0.Position = Vector3.Transform(fragInput0.Position, renderingContext.ProjectionClip) - Vector3.UnitZ * 0.0001f;
                    fragInput1.Position = Vector3.Transform(fragInput1.Position, renderingContext.ProjectionClip) - Vector3.UnitZ * 0.0001f;

                    Drawer.DrawLine(renderingContext, _shaderProgram, fragInput0, fragInput1);
                }
            }
        }
    }
}