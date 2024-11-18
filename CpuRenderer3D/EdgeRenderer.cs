using System.Numerics;

namespace CpuRenderer3D
{
    public class EdgeRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram _shaderProgram;

        public EdgeRenderer(Mesh mesh, IShaderProgram shaderProgram)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
        }

        public void Render(RenderingContext renderingContext)
        {
            Edge[] edges = _mesh.GetEdges();
            for (int eid = 0; eid < edges.Length; eid++)
            {
                VertexInput edgeV0 = new VertexInput(_mesh.GetVertex(edges[eid].V0), new Vector3(), new Vector4(0.8f, 0.8f, 0.8f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                VertexInput edgeV1 = new VertexInput(_mesh.GetVertex(edges[eid].V1), new Vector3(), new Vector4(0.8f, 0.8f, 0.8f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());

                FragmentInput fragInput0 = _shaderProgram.ComputeVertex(edgeV0, renderingContext);
                FragmentInput fragInput1 = _shaderProgram.ComputeVertex(edgeV1, renderingContext);

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