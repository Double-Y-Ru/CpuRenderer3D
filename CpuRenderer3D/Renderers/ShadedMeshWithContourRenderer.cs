using System.Collections.Generic;
using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ShadedMeshWithContourRenderer<TFragmentData> : IRenderer where TFragmentData : struct
    {
        private record struct TriangleVertexKey(int TriangleId, int VertexId);

        private readonly Mesh _mesh;
        private readonly IShaderProgram<TFragmentData> _shaderProgram;
        private readonly IInterpolator<FragmentInput<TFragmentData>> _interpolator;
        private readonly Vector4 _contourColor;

        private readonly Dictionary<TriangleVertexKey, Vector4> _triangleVerticesCache;
        private readonly Vector3[] _triangleNormalsCache;
        private Buffer<bool>? _mask;

        public ShadedMeshWithContourRenderer(Mesh mesh, IShaderProgram<TFragmentData> shaderProgram, IInterpolator<FragmentInput<TFragmentData>> interpolator, Vector4 contourColor)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _interpolator = interpolator;
            _contourColor = contourColor;
            _triangleVerticesCache = new Dictionary<TriangleVertexKey, Vector4>(_mesh.GetVertices().Length * 3);
            _triangleNormalsCache = new Vector3[_mesh.GetTriangles().Length];
        }

        public void Render(RenderingContext renderingContext)
        {
            Bounds screenBounds = new Bounds(Vector2.Zero, new Vector2(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height));
            Bounds objectBounds = new Bounds(new Vector2(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height), Vector2.Zero);

            if (_mask == null
             || _mask.Width != renderingContext.ColorBuffer.Width
             || _mask.Height != renderingContext.ColorBuffer.Height)
            {
                _mask = new Buffer<bool>(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height, false);
            }

            _mask.Clear();

            for (int tid = 0; tid < _mesh.GetTriangles().Length; tid++)
            {
                Triangle triangle = _mesh.GetTriangles()[tid];

                VertexInput vertInput0 = new VertexInput(_mesh.GetVertex(triangle.Vertex0.VertexIndex), _mesh.GetNormal(triangle.Vertex0.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex0.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput1 = new VertexInput(_mesh.GetVertex(triangle.Vertex1.VertexIndex), _mesh.GetNormal(triangle.Vertex1.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex1.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput2 = new VertexInput(_mesh.GetVertex(triangle.Vertex2.VertexIndex), _mesh.GetNormal(triangle.Vertex2.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex2.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());

                FragmentInput<TFragmentData> fragInput0 = _shaderProgram.ComputeVertex(vertInput0, renderingContext);
                FragmentInput<TFragmentData> fragInput1 = _shaderProgram.ComputeVertex(vertInput1, renderingContext);
                FragmentInput<TFragmentData> fragInput2 = _shaderProgram.ComputeVertex(vertInput2, renderingContext);

                Vector4 point0Screen = Vector4.Transform(fragInput0.Position, renderingContext.ClipScreen);
                Vector4 point1Screen = Vector4.Transform(fragInput1.Position, renderingContext.ClipScreen);
                Vector4 point2Screen = Vector4.Transform(fragInput2.Position, renderingContext.ClipScreen);

                _triangleVerticesCache[new TriangleVertexKey(tid, triangle.Vertex0.VertexIndex)] = point0Screen;
                _triangleVerticesCache[new TriangleVertexKey(tid, triangle.Vertex1.VertexIndex)] = point1Screen;
                _triangleVerticesCache[new TriangleVertexKey(tid, triangle.Vertex2.VertexIndex)] = point2Screen;

                _triangleNormalsCache[tid] = Vector3.Cross(fragInput0.Position.XYZ() - fragInput1.Position.XYZ(),
                                      fragInput0.Position.XYZ() - fragInput2.Position.XYZ());

                Rasterizer.DrawTriangle(point0Screen, fragInput0, point1Screen, fragInput1, point2Screen, fragInput2, _interpolator, screenBounds, TestDepthF, SetDepthF, SetColorAndMask);
            }

            for (int eid = 0; eid < _mesh.GetEdges().Length; eid++)
            {
                Edge edge = _mesh.GetEdges()[eid];

                if (edge.Tris.Length == 1)
                {
                    bool triangle0IsCameraFaced = Vector3.Dot(_triangleNormalsCache[edge.Tris[0]], Vector3.UnitZ) > 0;

                    if (triangle0IsCameraFaced)
                    {
                        Vector4 point0Screen = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[0], edge.Vertex0Index)];
                        Vector4 point1Screen = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[0], edge.Vertex1Index)];

                        Vector3 point0ScreenDivW = point0Screen.XYZDivW();
                        Vector3 point1ScreenDivW = point1Screen.XYZDivW();

                        CheckAndDrawLine(point0ScreenDivW, point1ScreenDivW);
                    }
                }
                else if (edge.Tris.Length == 2)
                {
                    bool triangle0IsCameraFaced = Vector3.Dot(_triangleNormalsCache[edge.Tris[0]], Vector3.UnitZ) > 0;
                    bool triangle1IsCameraFaced = Vector3.Dot(_triangleNormalsCache[edge.Tris[1]], Vector3.UnitZ) > 0;

                    if (triangle0IsCameraFaced != triangle1IsCameraFaced)
                    {
                        Vector4 tri0point0Screen = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[0], edge.Vertex0Index)];
                        Vector4 tri0point1Screen = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[0], edge.Vertex1Index)];

                        Vector4 tri1point0Screen = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[1], edge.Vertex0Index)];
                        Vector4 tri1point1Screen = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[1], edge.Vertex1Index)];

                        Vector3 tri0point0ScreenDivW = tri0point0Screen.XYZDivW();
                        Vector3 tri0point1ScreenDivW = tri0point1Screen.XYZDivW();

                        Vector3 tri1point0ScreenDivW = tri1point0Screen.XYZDivW();
                        Vector3 tri1point1ScreenDivW = tri1point1Screen.XYZDivW();

                        if (tri0point0ScreenDivW == tri1point0ScreenDivW && tri0point1ScreenDivW == tri1point1ScreenDivW)
                        {
                            CheckAndDrawLine(tri0point0ScreenDivW, tri0point1ScreenDivW);
                        }
                        else
                        {
                            CheckAndDrawLine(tri0point0ScreenDivW, tri0point1ScreenDivW);
                            CheckAndDrawLine(tri1point0ScreenDivW, tri1point1ScreenDivW);
                        }
                    }
                }
                else
                {
                    foreach (int triangleId in edge.Tris)
                    {
                        Vector4 point0Screen = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[0], edge.Vertex0Index)];
                        Vector4 point1Screen = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[0], edge.Vertex1Index)];

                        Vector3 point0ScreenDivW = point0Screen.XYZDivW();
                        Vector3 point1ScreenDivW = point1Screen.XYZDivW();

                        CheckAndDrawLine(point0ScreenDivW, point1ScreenDivW);
                    }
                }
            }

            for (int y = (int)objectBounds.Min.Y - 1; y <= (int)objectBounds.Max.Y; ++y)
            {
                for (int x = (int)objectBounds.Min.X - 1; x <= (int)objectBounds.Max.X; ++x)
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

            void CheckAndDrawLine(Vector3 triangleVertex0ClipDivW, Vector3 triangleVertex1ClipDivW)
            {
                if (screenBounds.IsInside(triangleVertex0ClipDivW.XY())
                 && screenBounds.IsInside(triangleVertex1ClipDivW.XY()))
                {
                    Rasterizer.DrawLine(triangleVertex0ClipDivW, triangleVertex1ClipDivW, _contourColor, TestDepthAndMask, (x, y, d) => { }, SetColor);
                }
            }

            void SetColorAndMask(int x, int y, FragmentInput<TFragmentData> fragmentInput)
            {
                _mask!.Set(x, y, true);
                objectBounds.Expand(x, y);

                Vector4 color = _shaderProgram.ComputeColor(fragmentInput, renderingContext);
                SetColor(x, y, color);
            }

            bool TestDepthAndMask(int x, int y, float depth)
            {
                return TestDepthF(x, y, depth - 0.001f) && _mask!.TryGet(x, y, out bool isInMask) && isInMask;
            }

            void SetColor(int x, int y, Vector4 color)
            {
                renderingContext.ColorBuffer.Set(x, y, color);
            }

            bool TestDepthF(int x, int y, float depth)
            {
                return renderingContext.DepthBuffer.TryGet(x, y, out float depthFromBuffer) && depth <= depthFromBuffer;
            }

            void SetDepthF(int x, int y, float depth)
            {
                renderingContext.DepthBuffer.Set(x, y, depth);
            }
        }
    }
}