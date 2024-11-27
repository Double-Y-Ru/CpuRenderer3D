using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ColoredMeshRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly Vector4 _fillColor;
        private readonly ColorInterpolator _colorInterpolator;

        public ColoredMeshRenderer(Mesh mesh, Vector4 fillColor)
        {
            _mesh = mesh;
            _fillColor = fillColor;
            _colorInterpolator = new ColorInterpolator();
        }

        public void Render(RenderingContext renderingContext)
        {
            Bounds3 screenBounds = new Bounds3(Vector3.Zero, new Vector3(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height, 1f));

            for (int i = 0; i < _mesh.GetTriangles().Length; i++)
            {
                Triangle triangle = _mesh.GetTriangles()[i];

                Vector3 triangleVertex0Local = _mesh.GetVertex(triangle.Vertex0.VertexIndex);
                Vector3 triangleVertex1Local = _mesh.GetVertex(triangle.Vertex1.VertexIndex);
                Vector3 triangleVertex2Local = _mesh.GetVertex(triangle.Vertex2.VertexIndex);

                Vector4 triangleVertex0Screen = Vector4.Transform(triangleVertex0Local, renderingContext.ModelScreen);
                Vector4 triangleVertex1Screen = Vector4.Transform(triangleVertex1Local, renderingContext.ModelScreen);
                Vector4 triangleVertex2Screen = Vector4.Transform(triangleVertex2Local, renderingContext.ModelScreen);

                Rasterizer.DrawTriangle(triangleVertex0Screen, _fillColor, triangleVertex1Screen, _fillColor, triangleVertex2Screen, _fillColor, _colorInterpolator, screenBounds, TestDepth, SetDepth, SetColor);
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
            }
        }
    }
}