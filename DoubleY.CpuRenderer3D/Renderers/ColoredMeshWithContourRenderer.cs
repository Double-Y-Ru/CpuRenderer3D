using System.Numerics;

namespace DoubleY.CpuRenderer3D.Renderers
{
    public class ColoredMeshWithContourRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly Vector4 _fillColor;
        private readonly Vector4 _contourColor;
        private Buffer<bool>? _mask;
        private readonly ColorInterpolator _colorInterpolator;

        private readonly Vector4[] _vertexScreenCache;
        private readonly bool[] _normalOrientationCache;

        public ColoredMeshWithContourRenderer(Mesh mesh, Vector4 fillColor, Vector4 contourColor)
        {
            _mesh = mesh;
            _fillColor = fillColor;
            _contourColor = contourColor;
            _mask = null;
            _colorInterpolator = new ColorInterpolator();
            _vertexScreenCache = new Vector4[_mesh.GetVertices().Length];
            _normalOrientationCache = new bool[_mesh.GetTriangles().Length];
        }

        public void Render(RenderingContext renderingContext)
        {
            Bounds3 screenBounds = new Bounds3(Vector3.Zero, new Vector3(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height, 1f));

            if (_mask == null
             || _mask.Width != renderingContext.ColorBuffer.Width
             || _mask.Height != renderingContext.ColorBuffer.Height)
            {
                _mask = new Buffer<bool>(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height, false);
            }

            _mask.Clear();

            int minX = _mask.Width;
            int minY = _mask.Height;
            int maxX = 0;
            int maxY = 0;

            for (int vid = 0; vid < _mesh.GetVertices().Length; vid++)
            {
                Vector3 triangleVertexLocal = _mesh.GetVertex(vid);
                Vector4 triangleVertexScreen = Vector4.Transform(triangleVertexLocal, renderingContext.ModelScreen);

                _vertexScreenCache[vid] = triangleVertexScreen;
            }

            for (int tid = 0; tid < _mesh.GetTriangles().Length; tid++)
            {
                Triangle triangle = _mesh.GetTriangles()[tid];

                Vector4 triangleVertex0Screen = _vertexScreenCache[triangle.Vertex0.VertexIndex];
                Vector4 triangleVertex1Screen = _vertexScreenCache[triangle.Vertex1.VertexIndex];
                Vector4 triangleVertex2Screen = _vertexScreenCache[triangle.Vertex2.VertexIndex];

                _normalOrientationCache[tid] = VectorExtenstions.Cross(
                    triangleVertex0Screen.XYDivW() - triangleVertex1Screen.XYDivW(),
                    triangleVertex0Screen.XYDivW() - triangleVertex2Screen.XYDivW()) > 0;

                Rasterizer.DrawTriangle(triangleVertex0Screen, _fillColor, triangleVertex1Screen, _fillColor, triangleVertex2Screen, _fillColor, _colorInterpolator, screenBounds, TestDepth, SetDepthAndMask, SetColor);
            }

            for (int y = minY - 1; y <= maxY; ++y)
            {
                for (int x = minX - 1; x <= maxX; ++x)
                {
                    if (!_mask.TryGet(x, y, out bool centerIsInside)) continue;

                    bool rightExists = _mask.TryGet(x + 1, y, out bool rightIsInside);
                    bool topExists = _mask.TryGet(x, y + 1, out bool topIsInside);

                    // Check outer borders
                    if (centerIsInside)
                    {
                        if (rightExists && !rightIsInside) SetColor(x, y, _contourColor);
                        if (topExists && !topIsInside) SetColor(x, y, _contourColor);
                    }
                    else
                    {
                        if (rightExists && rightIsInside) SetColor(x + 1, y, _contourColor);
                        if (topExists && topIsInside) SetColor(x, y + 1, _contourColor);
                    }
                }
            }

            for (int eid = 0; eid < _mesh.GetEdges().Length; eid++)
            {
                Edge edge = _mesh.GetEdges()[eid];

                if (edge.Tris.Length == 2)
                {
                    bool triangle0IsCameraFaced = _normalOrientationCache[edge.Tris[0]];
                    bool triangle1IsCameraFaced = _normalOrientationCache[edge.Tris[1]];

                    if (triangle0IsCameraFaced != triangle1IsCameraFaced)
                    {
                        Vector4 triangleVertex0Screen = _vertexScreenCache[edge.Vertex0Index];
                        Vector4 triangleVertex1Screen = _vertexScreenCache[edge.Vertex1Index];

                        Rasterizer.DrawLine(triangleVertex0Screen, triangleVertex1Screen, _contourColor, screenBounds, TestDepthAndMask, (x, y, d) => { }, SetColor);
                    }
                }
                else
                {
                    Vector4 triangleVertex0Screen = _vertexScreenCache[edge.Vertex0Index];
                    Vector4 triangleVertex1Screen = _vertexScreenCache[edge.Vertex1Index];

                    Rasterizer.DrawLine(triangleVertex0Screen, triangleVertex1Screen, _contourColor, screenBounds, TestDepthAndMask, (x, y, d) => { }, SetColor);
                }
            }

            bool TestDepth(int x, int y, float depth)
            {
                return renderingContext.DepthBuffer.TryGet(x, y, out float depthFromBuffer) && depth <= depthFromBuffer;
            }

            bool TestDepthAndMask(int x, int y, float depth)
            {
                return TestDepth(x, y, depth - 0.001f) && _mask!.TryGet(x, y, out bool isInMask) && isInMask;
            }

            void SetDepthAndMask(int x, int y, float depth)
            {
                renderingContext.DepthBuffer.Set(x, y, depth);

                _mask!.Set(x, y, true);

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }

            void SetColor(int x, int y, Vector4 color)
            {
                renderingContext.ColorBuffer.Set(x, y, color);
            }
        }

    }
}