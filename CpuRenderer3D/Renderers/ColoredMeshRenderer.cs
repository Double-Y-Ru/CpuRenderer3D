using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ColoredMeshRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly Vector4 _fillColor;

        public ColoredMeshRenderer(Mesh mesh, Vector4 fillColor)
        {
            _mesh = mesh;
            _fillColor = fillColor;
        }

        public void Render(RenderingContext renderingContext)
        {
            for (int i = 0; i < _mesh.GetTriangles().Length; i++)
            {
                Triangle triangle = _mesh.GetTriangles()[i];

                Vector3 triangleVertex0Local = _mesh.GetVertex(triangle.Vertex0.VertexIndex);
                Vector3 triangleVertex1Local = _mesh.GetVertex(triangle.Vertex1.VertexIndex);
                Vector3 triangleVertex2Local = _mesh.GetVertex(triangle.Vertex2.VertexIndex);

                Vector3 triangleVertex0Proj = Vector4.Transform(triangleVertex0Local, renderingContext.ModelClip).XYZDivW();
                Vector3 triangleVertex1Proj = Vector4.Transform(triangleVertex1Local, renderingContext.ModelClip).XYZDivW();
                Vector3 triangleVertex2Proj = Vector4.Transform(triangleVertex2Local, renderingContext.ModelClip).XYZDivW();

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
                    triangleVertex0Proj = Vector3.Transform(triangleVertex0Proj, renderingContext.ClipScreen);
                    triangleVertex1Proj = Vector3.Transform(triangleVertex1Proj, renderingContext.ClipScreen);
                    triangleVertex2Proj = Vector3.Transform(triangleVertex2Proj, renderingContext.ClipScreen);

                    Rasterizer.DrawTriangle(triangleVertex0Proj, triangleVertex1Proj, triangleVertex2Proj, _fillColor, TestDepth, SetDepth, SetColor);
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
            }
        }
    }
}