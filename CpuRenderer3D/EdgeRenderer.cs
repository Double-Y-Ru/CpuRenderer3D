using System.Numerics;

namespace CpuRenderer3D
{
    public class EdgeRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram _shaderProgram;

        private readonly FragmentInput[] _fragVerticesCache;

        public EdgeRenderer(Mesh mesh, IShaderProgram shaderProgram)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _fragVerticesCache = new FragmentInput[_mesh.GetVertices().Length];
        }

        public void Render(RenderingContext renderingContext)
        {
            for (int vid = 0; vid < _mesh.GetVertices().Length; ++vid)
            {
                VertexInput vertInput = new VertexInput(_mesh.GetVertex(vid), new Vector3(), new Vector4(0.5f, 0.5f, 0.5f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                _fragVerticesCache[vid] = _shaderProgram.ComputeVertex(vertInput, renderingContext);
            }

            for (int eid = 0; eid < _mesh.GetEdges().Length; eid++)
            {
                Edge edge = _mesh.GetEdges()[eid];

                FragmentInput fragInput0 = _fragVerticesCache[edge.Vertex0Index];
                FragmentInput fragInput1 = _fragVerticesCache[edge.Vertex1Index];

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