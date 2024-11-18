using System.Numerics;

namespace CpuRenderer3D
{
    public class MeshRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram _shaderProgram;

        public MeshRenderer(Mesh mesh, IShaderProgram shaderProgram)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
        }

        public void Render(RenderingContext renderingContext)
        {
            Triangle[] triangles = _mesh.GetTriangles();

            for (int i = 0; i < triangles.Length; i++)
            {
                Triangle triangle = triangles[i];

                VertexInput vertInput0 = new VertexInput(_mesh.GetVertex(triangle.V0), new Vector3(), new Vector4(0.8f, 0.0f, 0.0f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput1 = new VertexInput(_mesh.GetVertex(triangle.V1), new Vector3(), new Vector4(0.0f, 0.8f, 0.0f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput2 = new VertexInput(_mesh.GetVertex(triangle.V2), new Vector3(), new Vector4(0.0f, 0.0f, 0.8f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());

                FragmentInput fragInput0 = _shaderProgram.ComputeVertex(vertInput0, renderingContext);
                FragmentInput fragInput1 = _shaderProgram.ComputeVertex(vertInput1, renderingContext);
                FragmentInput fragInput2 = _shaderProgram.ComputeVertex(vertInput2, renderingContext);

                Vector3 triangleNormalP = Vector3.Cross(
                    fragInput0.Position - fragInput1.Position,
                    fragInput0.Position - fragInput2.Position);

                if (Vector3.Dot(triangleNormalP, Vector3.UnitZ) < 0) continue;

                if (-1f < fragInput0.Position.X && fragInput0.Position.X < 1f
                 && -1f < fragInput0.Position.Y && fragInput0.Position.Y < 1f
                 && -1f < fragInput0.Position.Z && fragInput0.Position.Z < 1f
                 && -1f < fragInput1.Position.X && fragInput1.Position.X < 1f
                 && -1f < fragInput1.Position.Y && fragInput1.Position.Y < 1f
                 && -1f < fragInput1.Position.Z && fragInput1.Position.Z < 1f
                 && -1f < fragInput2.Position.X && fragInput2.Position.X < 1f
                 && -1f < fragInput2.Position.Y && fragInput2.Position.Y < 1f
                 && -1f < fragInput2.Position.Z && fragInput2.Position.Z < 1f
                 )
                {
                    fragInput0.Position = Vector3.Transform(fragInput0.Position, renderingContext.ProjectionClip);
                    fragInput1.Position = Vector3.Transform(fragInput1.Position, renderingContext.ProjectionClip);
                    fragInput2.Position = Vector3.Transform(fragInput2.Position, renderingContext.ProjectionClip);

                    DrawTriangle(renderingContext, _shaderProgram, fragInput0, fragInput1, fragInput2);
                }
            }
        }

        private void DrawTriangle(RenderingContext shaderContext, IShaderProgram shaderProgram,
            FragmentInput f0, FragmentInput f1, FragmentInput f2)
        {
            if (f0.Position.Y == f1.Position.Y
             && f0.Position.Y == f2.Position.Y)
                return;

            if (f0.Position.Y > f1.Position.Y) Swap(ref f0, ref f1);
            if (f0.Position.Y > f2.Position.Y) Swap(ref f0, ref f2);
            if (f1.Position.Y > f2.Position.Y) Swap(ref f1, ref f2);

            int lowerY = (int)float.Round(f0.Position.Y);
            int rightY = (int)float.Round(f1.Position.Y);
            int upperY = (int)float.Round(f2.Position.Y);

            float tLeft = 0f;
            float tLeftDelta = 1f / (upperY - lowerY + 1);

            float tRightLower = 0f;
            float tRightLowerDelta = 1f / (rightY - lowerY + 1);

            float tRightUpper = 0f;
            float tRightUpperDelta = 1f / (upperY - rightY + 1);

            for (int y = lowerY; y < rightY; ++y)
            {
                FragmentInput leftPoint = FragmentInput.Interpolate(f0, f2, tLeft);
                FragmentInput rightPoint = FragmentInput.Interpolate(f0, f1, tRightLower);

                DrawHorizontalLine(leftPoint, rightPoint, y);

                tLeft += tLeftDelta;
                tRightLower += tRightLowerDelta;
            }

            for (int y = rightY; y <= upperY; ++y)
            {
                FragmentInput leftPoint = FragmentInput.Interpolate(f0, f2, tLeft);
                FragmentInput rightPoint = FragmentInput.Interpolate(f1, f2, tRightUpper);

                DrawHorizontalLine(leftPoint, rightPoint, y);

                tLeft += tLeftDelta;
                tRightUpper += tRightUpperDelta;
            }

            void DrawHorizontalLine(FragmentInput lineStart, FragmentInput lineEnd, int y)
            {
                if (lineStart.Position.X > lineEnd.Position.X)
                    Swap(ref lineStart, ref lineEnd);

                int lineStartX = (int)float.Round(lineStart.Position.X);
                int lineEndX = (int)float.Round(lineEnd.Position.X);

                float t = 0;
                float dt = 1f / (lineEndX - lineStartX + 1);

                for (int x = lineStartX; x <= lineEndX; x++)
                {
                    FragmentInput pixel = FragmentInput.Interpolate(lineStart, lineEnd, t);
                    Vector4 pixelColor = shaderProgram.ComputeColor(pixel, shaderContext);

                    if (shaderContext.DepthBuffer.TryGet(x, y, out float zFromBuffer) && pixel.Position.Z <= zFromBuffer)
                    {
                        shaderContext.DepthBuffer.Set(x, y, pixel.Position.Z);
                        shaderContext.ColorBuffer.Set(x, y, pixelColor);
                    }

                    t += dt;
                }
            }

            void Swap(ref FragmentInput a, ref FragmentInput b)
            {
                FragmentInput c = a;
                a = b;
                b = c;
            }
        }
    }
}