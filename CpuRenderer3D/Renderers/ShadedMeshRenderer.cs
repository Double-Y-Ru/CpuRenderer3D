using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ShadedMeshRenderer<TFragmentData> : IRenderer where TFragmentData : struct
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram<TFragmentData> _shaderProgram;
        private readonly IInterpolator<TFragmentData> _interpolator;

        public ShadedMeshRenderer(Mesh mesh, IShaderProgram<TFragmentData> shaderProgram, IInterpolator<TFragmentData> interpolator)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _interpolator = interpolator;
        }

        public void Render(RenderingContext renderingContext)
        {
            for (int i = 0; i < _mesh.GetTriangles().Length; i++)
            {
                Triangle triangle = _mesh.GetTriangles()[i];

                VertexInput vertInput0 = new VertexInput(_mesh.GetVertex(triangle.Vertex0.VertexIndex), _mesh.GetNormal(triangle.Vertex0.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex0.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput1 = new VertexInput(_mesh.GetVertex(triangle.Vertex1.VertexIndex), _mesh.GetNormal(triangle.Vertex1.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex1.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput2 = new VertexInput(_mesh.GetVertex(triangle.Vertex2.VertexIndex), _mesh.GetNormal(triangle.Vertex2.NormalIndex), Vector4.One, _mesh.GetTexCoord(triangle.Vertex2.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());

                FragmentInput<TFragmentData> fragInput0 = _shaderProgram.ComputeVertex(vertInput0, renderingContext);
                FragmentInput<TFragmentData> fragInput1 = _shaderProgram.ComputeVertex(vertInput1, renderingContext);
                FragmentInput<TFragmentData> fragInput2 = _shaderProgram.ComputeVertex(vertInput2, renderingContext);

                Drawer.DrawTriangleBary(fragInput0, fragInput1, fragInput2, _interpolator, renderingContext.ClipScreen, TestPixel, SetPixel);
            }


            bool TestPixel(int x, int y, FragmentInput<TFragmentData> fragmentInput)
            {
                return renderingContext.DepthBuffer.TryGet(x, y, out float depthFromBuffer) && fragmentInput.Position.Z <= depthFromBuffer;
            }

            void SetPixel(int x, int y, FragmentInput<TFragmentData> fragmentInput)
            {
                renderingContext.DepthBuffer.Set(x, y, fragmentInput.Position.Z);

                Vector4 color = _shaderProgram.ComputeColor(fragmentInput, renderingContext);
                renderingContext.ColorBuffer.Set(x, y, color);
            }
        }
    }
}