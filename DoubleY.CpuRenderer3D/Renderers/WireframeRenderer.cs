using System.Numerics;

namespace DoubleY.CpuRenderer3D.Renderers
{
    public class WireframeRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly Vector4 _wireframeColor;
        private readonly Vector4[] _verticesCache;

        public WireframeRenderer(Mesh mesh, Vector4 wireframeColor)
        {
            _mesh = mesh;
            _wireframeColor = wireframeColor;
            _verticesCache = new Vector4[_mesh.GetVertices().Length];
        }

        public void Render(RenderingContext renderingContext)
        {
            Bounds3 clipBounds = new Bounds3(Vector3.Zero, new Vector3(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height, 1f));

            for (int vid = 0; vid < _mesh.GetVertices().Length; ++vid)
            {
                Vector3 vertexLocal = _mesh.GetVertex(vid);
                Vector4 vertexScreen = Vector4.Transform(vertexLocal, renderingContext.ModelScreen);
                _verticesCache[vid] = vertexScreen;
            }

            for (int eid = 0; eid < _mesh.GetEdges().Length; eid++)
            {
                Edge edge = _mesh.GetEdges()[eid];

                Vector4 vertex0Screen = _verticesCache[edge.Vertex0Index];
                Vector4 vertex1Screen = _verticesCache[edge.Vertex1Index];

                Rasterizer.DrawLine(vertex0Screen, vertex1Screen, _wireframeColor, clipBounds, TestDepth, SetDepth, SetColor);
            }

            bool TestDepth(int x, int y, float depth)
            {
                return renderingContext.DepthBuffer.TryGet(x, y, out float depthFromBuffer) && depth <= depthFromBuffer;
            }

            void SetDepth(int x, int y, float depth) => renderingContext.DepthBuffer.Set(x, y, depth);

            void SetColor(int x, int y, Vector4 color) => renderingContext.ColorBuffer.Set(x, y, color);
        }
    }
}