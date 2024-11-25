using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class EdgeRenderer<TFragmentData> : IRenderer where TFragmentData : struct
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram<TFragmentData> _shaderProgram;
        private readonly IInterpolator<TFragmentData> _interpolator;

        private readonly FragmentInput<TFragmentData>[] _fragVerticesCache;

        public EdgeRenderer(Mesh mesh, IShaderProgram<TFragmentData> shaderProgram, IInterpolator<TFragmentData> interpolator)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _interpolator = interpolator;
            _fragVerticesCache = new FragmentInput<TFragmentData>[_mesh.GetVertices().Length];
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

                FragmentInput<TFragmentData> fragInput0 = _fragVerticesCache[edge.Vertex0Index];
                FragmentInput<TFragmentData> fragInput1 = _fragVerticesCache[edge.Vertex1Index];

                if (-1f < fragInput0.Position.X && fragInput0.Position.X < 1f
                 && -1f < fragInput0.Position.Y && fragInput0.Position.Y < 1f
                 && -1f < fragInput0.Position.Z && fragInput0.Position.Z < 1f
                 && -1f < fragInput1.Position.X && fragInput1.Position.X < 1f
                 && -1f < fragInput1.Position.Y && fragInput1.Position.Y < 1f
                 && -1f < fragInput1.Position.Z && fragInput1.Position.Z < 1f)
                {
                    fragInput0.Position = Vector3.Transform(fragInput0.Position, renderingContext.ClipScreen) - Vector3.UnitZ * 0.0001f;
                    fragInput1.Position = Vector3.Transform(fragInput1.Position, renderingContext.ClipScreen) - Vector3.UnitZ * 0.0001f;

                    Drawer.DrawLine(fragInput0, fragInput1, _interpolator, TestDepth, SetDepth, SetColor);
                }
            }

            bool TestDepth(int x, int y, float depth)
            {
                return renderingContext.DepthBuffer.TryGet(x, y, out float depthFromBuffer) && depth <= depthFromBuffer;
            }

            void SetDepth(int x, int y, float depth)
            {
                renderingContext.DepthBuffer.Set(x, y, depth);
            }

            void SetColor(int x, int y, FragmentInput<TFragmentData> fragInput)
            {
                Vector4 color = _shaderProgram.ComputeColor(fragInput, renderingContext);

                renderingContext.ColorBuffer.Set(x, y, color);
            }
        }
    }
}