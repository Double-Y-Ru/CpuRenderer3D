using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ColoredMeshWithContourRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly Vector4 _fillColor;
        private readonly Vector4 _contourColor;

        private Buffer<bool>? _stencilBuffer;
        private readonly float _depthThreshold;

        public ColoredMeshWithContourRenderer(Mesh mesh, Vector4 fillColor, Vector4 contourColor, float depthThreshold)
        {
            _mesh = mesh;
            _fillColor = fillColor;
            _contourColor = contourColor;
            _stencilBuffer = null;
            _depthThreshold = depthThreshold;
        }

        public void Render(RenderingContext renderingContext)
        {
            if (_stencilBuffer == null
             || _stencilBuffer.Width != renderingContext.ColorBuffer.Width
             || _stencilBuffer.Height != renderingContext.ColorBuffer.Height)
            {
                _stencilBuffer = new Buffer<bool>(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height, false);
            }

            _stencilBuffer.Clear();

            int minX = _stencilBuffer.Width;
            int minY = _stencilBuffer.Height;
            int maxX = 0;
            int maxY = 0;

            for (int i = 0; i < _mesh.GetTriangles().Length; i++)
            {
                Triangle triangle = _mesh.GetTriangles()[i];

                Vector3 triangleVertex0Local = _mesh.GetVertex(triangle.Vertex0.VertexIndex);
                Vector3 triangleVertex1Local = _mesh.GetVertex(triangle.Vertex1.VertexIndex);
                Vector3 triangleVertex2Local = _mesh.GetVertex(triangle.Vertex2.VertexIndex);

                Vector3 triangleVertex0Proj = Vector4.Transform(triangleVertex0Local, renderingContext.ModelProjection).XYZDivW();
                Vector3 triangleVertex1Proj = Vector4.Transform(triangleVertex1Local, renderingContext.ModelProjection).XYZDivW();
                Vector3 triangleVertex2Proj = Vector4.Transform(triangleVertex2Local, renderingContext.ModelProjection).XYZDivW();

                Vector3 triangleNormalP = Vector3.Cross(
                    triangleVertex0Proj - triangleVertex1Proj,
                    triangleVertex0Proj - triangleVertex2Proj);

                if (Vector3.Dot(triangleNormalP, Vector3.UnitZ) < 0) continue;

                if (-1f < triangleVertex0Proj.X && triangleVertex0Proj.X < 1f
                 && -1f < triangleVertex0Proj.Y && triangleVertex0Proj.Y < 1f
                 && -1f < triangleVertex0Proj.Z && triangleVertex0Proj.Z < 1f
                 && -1f < triangleVertex1Proj.X && triangleVertex1Proj.X < 1f
                 && -1f < triangleVertex1Proj.Y && triangleVertex1Proj.Y < 1f
                 && -1f < triangleVertex1Proj.Z && triangleVertex1Proj.Z < 1f
                 && -1f < triangleVertex2Proj.X && triangleVertex2Proj.X < 1f
                 && -1f < triangleVertex2Proj.Y && triangleVertex2Proj.Y < 1f
                 && -1f < triangleVertex2Proj.Z && triangleVertex2Proj.Z < 1f
                 )
                {
                    triangleVertex0Proj = Vector3.Transform(triangleVertex0Proj, renderingContext.ProjectionClip);
                    triangleVertex1Proj = Vector3.Transform(triangleVertex1Proj, renderingContext.ProjectionClip);
                    triangleVertex2Proj = Vector3.Transform(triangleVertex2Proj, renderingContext.ProjectionClip);

                    Drawer.DrawTriangle(triangleVertex0Proj, triangleVertex1Proj, triangleVertex2Proj, _fillColor, TestDepth, SetDepth, SetColor);
                }
            }

            for (int y = minY - 1; y < maxY; ++y)
            {
                for (int x = minX - 1; x < maxX; ++x)
                {
                    if (!_stencilBuffer.TryGet(x, y, out bool centerIsInside)) continue;

                    bool rightExists = _stencilBuffer.TryGet(x + 1, y, out bool rightIsInside);
                    bool topExists = _stencilBuffer.TryGet(x, y + 1, out bool topIsInside);

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

                        float horizontalDiff = (rightDepth - centerDepth) / centerDepth;
                        float verticalDiff = (topDepth - centerDepth) / centerDepth;

                        if (horizontalDiff > _depthThreshold) SetColor(x, y, _contourColor);
                        else if (horizontalDiff < -_depthThreshold) SetColor(x + 1, y, _contourColor);

                        if (verticalDiff > _depthThreshold) SetColor(x, y, _contourColor);
                        else if (verticalDiff < -_depthThreshold) SetColor(x, y + 1, _contourColor);
                    }
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

            void SetColor(int x, int y, Vector4 color)
            {
                renderingContext.ColorBuffer.Set(x, y, color);
                _stencilBuffer.Set(x, y, true);

                if (x < minX) minX = x;
                if (y < minY) minY = y;
                if (x > maxX) maxX = x;
                if (y > maxY) maxY = y;
            }
        }
    }
}