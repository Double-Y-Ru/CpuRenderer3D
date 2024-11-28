using System.Collections.Generic;
using System.Numerics;

namespace DoubleY.CpuRenderer3D.Renderers
{
    public class ShadedMeshWithDepthBasedContourRenderer<TFragmentData> : IRenderer where TFragmentData : struct
    {
        private record struct TriangleVertexKey(int TriangleId, int VertexId);

        private readonly Mesh _mesh;
        private readonly IShaderProgram<TFragmentData> _shaderProgram;
        private readonly IInterpolator<FragmentInput<TFragmentData>> _interpolator;
        private readonly Vector4 _contourColor;

        private readonly Dictionary<TriangleVertexKey, Vector4> _triangleVerticesScreen;
        private readonly bool[] _normalOrientationCache;
        private Buffer<bool>? _mask;
        private float _depthThreshold;

        public ShadedMeshWithDepthBasedContourRenderer(Mesh mesh, IShaderProgram<TFragmentData> shaderProgram, IInterpolator<FragmentInput<TFragmentData>> interpolator, Vector4 contourColor, float depthThreshold)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _interpolator = interpolator;
            _contourColor = contourColor;
            _triangleVerticesScreen = new Dictionary<TriangleVertexKey, Vector4>(_mesh.GetVertices().Length * 3);
            _normalOrientationCache = new bool[_mesh.GetTriangles().Length];
            _depthThreshold = depthThreshold;
        }

        public void Render(RenderingContext renderingContext)
        {
            Bounds3 screenBounds = new Bounds3(Vector3.Zero, new Vector3(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height, 1f));
            Bounds2 objectBounds = new Bounds2(new Vector2(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height), Vector2.Zero);

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

                _triangleVerticesScreen[new TriangleVertexKey(tid, triangle.Vertex0.VertexIndex)] = point0Screen;
                _triangleVerticesScreen[new TriangleVertexKey(tid, triangle.Vertex1.VertexIndex)] = point1Screen;
                _triangleVerticesScreen[new TriangleVertexKey(tid, triangle.Vertex2.VertexIndex)] = point2Screen;

                _normalOrientationCache[tid] = VectorExtenstions.Cross(fragInput0.Position.XY() - fragInput1.Position.XY(),
                                                                       fragInput0.Position.XY() - fragInput2.Position.XY()) > 0f;

                Rasterizer.DrawTriangle(point0Screen, fragInput0, point1Screen, fragInput1, point2Screen, fragInput2, _interpolator, screenBounds, TestDepthF, SetDepthF, SetColorAndMask);
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

                    // Check inner depth difference
                    if (centerIsInside && rightExists && topExists)
                    {
                        float centerDepth = renderingContext.DepthBuffer.Get(x, y);
                        float rightDepth = renderingContext.DepthBuffer.Get(x + 1, y);
                        float topDepth = renderingContext.DepthBuffer.Get(x, y + 1);

                        float horizontalDiff = (rightDepth - centerDepth) * centerDepth;
                        float verticalDiff = (topDepth - centerDepth) * centerDepth;

                        if (horizontalDiff > _depthThreshold) SetColor(x, y, _contourColor);
                        else if (horizontalDiff < -_depthThreshold) SetColor(x + 1, y, _contourColor);

                        if (verticalDiff > _depthThreshold) SetColor(x, y, _contourColor);
                        else if (verticalDiff < -_depthThreshold) SetColor(x, y + 1, _contourColor);
                    }
                }
            }

            void CheckAndDrawLine(Vector3 triangleVertex0ClipDivW, Vector3 triangleVertex1ClipDivW)
            {
                if (screenBounds.Contains(triangleVertex0ClipDivW)
                 && screenBounds.Contains(triangleVertex1ClipDivW))
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