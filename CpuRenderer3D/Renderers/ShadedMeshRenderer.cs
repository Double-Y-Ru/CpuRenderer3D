using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ShadedMeshRenderer<TFragmentData> : IRenderer where TFragmentData : struct
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram<TFragmentData> _shaderProgram;
        private readonly IInterpolator<FragmentInput<TFragmentData>> _interpolator;

        public ShadedMeshRenderer(Mesh mesh, IShaderProgram<TFragmentData> shaderProgram, IInterpolator<FragmentInput<TFragmentData>> interpolator)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _interpolator = interpolator;
        }

        public void Render(RenderingContext renderingContext)
        {
            Bounds3 clipBounds = new Bounds3(Vector3.Zero, new Vector3(renderingContext.ColorBuffer.Width, renderingContext.ColorBuffer.Height, 1f));

            for (int i = 0; i < _mesh.GetTriangles().Length; i++)
            {
                Triangle triangle = _mesh.GetTriangles()[i];

                VertexInput vertInput0 = new VertexInput(_mesh.GetVertex(triangle.Vertex0.VertexIndex), _mesh.GetNormal(triangle.Vertex0.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex0.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput1 = new VertexInput(_mesh.GetVertex(triangle.Vertex1.VertexIndex), _mesh.GetNormal(triangle.Vertex1.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex1.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput2 = new VertexInput(_mesh.GetVertex(triangle.Vertex2.VertexIndex), _mesh.GetNormal(triangle.Vertex2.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex2.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());

                FragmentInput<TFragmentData> fragInput0 = _shaderProgram.ComputeVertex(vertInput0, renderingContext);
                FragmentInput<TFragmentData> fragInput1 = _shaderProgram.ComputeVertex(vertInput1, renderingContext);
                FragmentInput<TFragmentData> fragInput2 = _shaderProgram.ComputeVertex(vertInput2, renderingContext);

                Vector4 point0Screen = Vector4.Transform(fragInput0.Position, renderingContext.ClipScreen);
                Vector4 point1Screen = Vector4.Transform(fragInput1.Position, renderingContext.ClipScreen);
                Vector4 point2Screen = Vector4.Transform(fragInput2.Position, renderingContext.ClipScreen);

                Rasterizer.DrawTriangle(point0Screen, fragInput0, point1Screen, fragInput1, point2Screen, fragInput2, _interpolator, clipBounds, TestDepth, SetDepth, SetPixel);
            }

            bool TestDepth(int x, int y, float depth)
            {
                return renderingContext.DepthBuffer.TryGet(x, y, out float depthFromBuffer) && depth <= depthFromBuffer;
            }

            void SetDepth(int x, int y, float depth)
            {
                renderingContext.DepthBuffer.Set(x, y, depth);
            }

            void SetPixel(int x, int y, FragmentInput<TFragmentData> fragmentInput)
            {
                Vector4 color = _shaderProgram.ComputeColor(fragmentInput, renderingContext);
                renderingContext.ColorBuffer.Set(x, y, color);
            }
        }
    }
}