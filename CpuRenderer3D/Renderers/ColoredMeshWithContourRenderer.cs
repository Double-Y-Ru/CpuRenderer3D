using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ColoredMeshWithContourRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly Vector4 _fillColor;
        private readonly Vector4 _contourColor;

        private Buffer<bool>? _mask;

        private readonly Vector3[] _vertexCache;
        private readonly Vector3[] _normalCache;

        public ColoredMeshWithContourRenderer(Mesh mesh, Vector4 fillColor, Vector4 contourColor)
        {
            _mesh = mesh;
            _fillColor = fillColor;
            _contourColor = contourColor;
            _mask = null;
            _vertexCache = new Vector3[_mesh.GetVertices().Length];
            _normalCache = new Vector3[_mesh.GetTriangles().Length];
        }

        public void Render(RenderingContext renderingContext)
        {
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
                Vector3 triangleVertexClipDivW = Vector4.Transform(triangleVertexLocal, renderingContext.ModelClip).XYZDivW();

                _vertexCache[vid] = triangleVertexClipDivW;
            }

            for (int tid = 0; tid < _mesh.GetTriangles().Length; tid++)
            {
                Triangle triangle = _mesh.GetTriangles()[tid];

                Vector3 triangleVertex0ClipDivW = _vertexCache[triangle.Vertex0.VertexIndex];
                Vector3 triangleVertex1ClipDivW = _vertexCache[triangle.Vertex1.VertexIndex];
                Vector3 triangleVertex2ClipDivW = _vertexCache[triangle.Vertex2.VertexIndex];

                Vector3 triangleNormalClipDivW = Vector3.Cross(
                    triangleVertex0ClipDivW - triangleVertex1ClipDivW,
                    triangleVertex0ClipDivW - triangleVertex2ClipDivW);

                _normalCache[tid] = triangleNormalClipDivW;

                if (Vector3.Dot(triangleNormalClipDivW, Vector3.UnitZ) < 0) continue;

                if (-1f < triangleVertex0ClipDivW.X && triangleVertex0ClipDivW.X < 1f
                 && -1f < triangleVertex0ClipDivW.Y && triangleVertex0ClipDivW.Y < 1f
                 && -1f < triangleVertex0ClipDivW.Z && triangleVertex0ClipDivW.Z < 1f
                 && -1f < triangleVertex1ClipDivW.X && triangleVertex1ClipDivW.X < 1f
                 && -1f < triangleVertex1ClipDivW.Y && triangleVertex1ClipDivW.Y < 1f
                 && -1f < triangleVertex1ClipDivW.Z && triangleVertex1ClipDivW.Z < 1f
                 && -1f < triangleVertex2ClipDivW.X && triangleVertex2ClipDivW.X < 1f
                 && -1f < triangleVertex2ClipDivW.Y && triangleVertex2ClipDivW.Y < 1f
                 && -1f < triangleVertex2ClipDivW.Z && triangleVertex2ClipDivW.Z < 1f
                 )
                {
                    Vector3 triangleVertex0Screen = Vector3.Transform(triangleVertex0ClipDivW, renderingContext.ClipScreen);
                    Vector3 triangleVertex1Screen = Vector3.Transform(triangleVertex1ClipDivW, renderingContext.ClipScreen);
                    Vector3 triangleVertex2Screen = Vector3.Transform(triangleVertex2ClipDivW, renderingContext.ClipScreen);

                    Drawer.DrawTriangle(triangleVertex0Screen, triangleVertex1Screen, triangleVertex2Screen, _fillColor, TestDepth, SetDepthAndMask, SetColor);
                }
            }

            for (int y = minY - 1; y < maxY; ++y)
            {
                for (int x = minX - 1; x < maxX; ++x)
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
                    bool triangle0IsCameraFaced = Vector3.Dot(_normalCache[edge.Tris[0]], Vector3.UnitZ) > 0;
                    bool triangle1IsCameraFaced = Vector3.Dot(_normalCache[edge.Tris[1]], Vector3.UnitZ) > 0;

                    if (!triangle0IsCameraFaced && !triangle1IsCameraFaced) continue;
                    if (triangle0IsCameraFaced == triangle1IsCameraFaced) continue;

                    Vector3 triangleVertex0ClipDivW = _vertexCache[edge.Vertex0Index];
                    Vector3 triangleVertex1ClipDivW = _vertexCache[edge.Vertex1Index];

                    CheckAndDrawLine(triangleVertex0ClipDivW, triangleVertex1ClipDivW);
                }
                else
                {
                    Vector3 triangleVertex0ClipDivW = _vertexCache[edge.Vertex0Index];
                    Vector3 triangleVertex1ClipDivW = _vertexCache[edge.Vertex1Index];

                    CheckAndDrawLine(triangleVertex0ClipDivW, triangleVertex1ClipDivW);
                }
            }

            void CheckAndDrawLine(Vector3 triangleVertex0ClipDivW, Vector3 triangleVertex1ClipDivW)
            {
                if (-1f < triangleVertex0ClipDivW.X && triangleVertex0ClipDivW.X < 1f
                 && -1f < triangleVertex0ClipDivW.Y && triangleVertex0ClipDivW.Y < 1f
                 && -1f < triangleVertex0ClipDivW.Z && triangleVertex0ClipDivW.Z < 1f
                 && -1f < triangleVertex1ClipDivW.X && triangleVertex1ClipDivW.X < 1f
                 && -1f < triangleVertex1ClipDivW.Y && triangleVertex1ClipDivW.Y < 1f
                 && -1f < triangleVertex1ClipDivW.Z && triangleVertex1ClipDivW.Z < 1f)
                {
                    Vector3 triangleVertex0Screen = Vector3.Transform(triangleVertex0ClipDivW, renderingContext.ClipScreen);
                    Vector3 triangleVertex1Screen = Vector3.Transform(triangleVertex1ClipDivW, renderingContext.ClipScreen);

                    Drawer.DrawLine(triangleVertex0Screen, triangleVertex1Screen, _contourColor, TestDepthAndMask, (x, y, d) => { }, SetColor);
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