using System.Numerics;

namespace CpuRenderer3D
{
    public class EdgeRenderer : IRenderer
    {
        private readonly Mesh _mesh;
        private readonly IShaderProgram _shaderProgram;

        public EdgeRenderer(Mesh mesh, IShaderProgram shaderProgram)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
        }

        public void Render(RenderingContext renderingContext)
        {
            Edge[] edges = _mesh.GetEdges();
            for (int eid = 0; eid < edges.Length; eid++)
            {
                VertexInput edgeV0 = new VertexInput(_mesh.GetVertex(edges[eid].V0), new Vector3(), new Vector4(0.8f, 0.8f, 0.8f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                VertexInput edgeV1 = new VertexInput(_mesh.GetVertex(edges[eid].V1), new Vector3(), new Vector4(0.8f, 0.8f, 0.8f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());

                FragmentInput fragInput0 = _shaderProgram.ComputeVertex(edgeV0, renderingContext);
                FragmentInput fragInput1 = _shaderProgram.ComputeVertex(edgeV1, renderingContext);

                if (-1f < fragInput0.Position.X && fragInput0.Position.X < 1f
                 && -1f < fragInput0.Position.Y && fragInput0.Position.Y < 1f
                 && -1f < fragInput0.Position.Z && fragInput0.Position.Z < 1f
                 && -1f < fragInput1.Position.X && fragInput1.Position.X < 1f
                 && -1f < fragInput1.Position.Y && fragInput1.Position.Y < 1f
                 && -1f < fragInput1.Position.Z && fragInput1.Position.Z < 1f)
                {
                    fragInput0.Position = Vector3.Transform(fragInput0.Position, renderingContext.ProjectionClip) - Vector3.UnitZ * 0.0001f;
                    fragInput1.Position = Vector3.Transform(fragInput1.Position, renderingContext.ProjectionClip) - Vector3.UnitZ * 0.0001f;

                    DrawLine(renderingContext, _shaderProgram, fragInput0, fragInput1);
                }
            }
        }

        private void DrawLine(RenderingContext shaderContext, IShaderProgram shaderProgram, FragmentInput f0, FragmentInput f1)
        {
            bool isGentle = true;

            if (Math.Abs(f0.Position.X - f1.Position.X) < Math.Abs(f0.Position.Y - f1.Position.Y))
            {
                isGentle = false;

                Vector3 f0Pos = new Vector3(f0.Position.Y, f0.Position.X, f0.Position.Z);
                Vector3 f1Pos = new Vector3(f1.Position.Y, f1.Position.X, f1.Position.Z);

                f0.Position = f0Pos;
                f1.Position = f1Pos;
            }

            if (f0.Position.X > f1.Position.X)
            {
                Swap(ref f0, ref f1);
            }

            int gentleLeftX = (int)float.Round(f0.Position.X);
            int gentleLeftY = (int)float.Round(f0.Position.Y);

            int gentleRightX = (int)float.Round(f1.Position.X);
            int gentleRightY = (int)float.Round(f1.Position.Y);

            int width = gentleRightX - gentleLeftX;
            int height = gentleRightY - gentleLeftY;

            float t = 0;
            float dt = 1f / width;

            int derror2 = Math.Abs(height) * 2;
            int error2 = 0;

            int y = gentleLeftY;
            for (int x = gentleLeftX; x <= gentleRightX; x++)
            {
                FragmentInput pixel = FragmentInput.Interpolate(f0, f1, t);
                Vector4 pixelColor = shaderProgram.ComputeColor(pixel, shaderContext);

                if (isGentle)
                {
                    if (shaderContext.DepthBuffer.TryGet(x, y, out float zFromBuffer) && pixel.Position.Z <= zFromBuffer)
                    {
                        shaderContext.DepthBuffer.Set(x, y, pixel.Position.Z);
                        shaderContext.ColorBuffer.Set(x, y, pixelColor);
                    }
                }
                else
                {
                    if (shaderContext.DepthBuffer.TryGet(y, x, out float zFromBuffer) && pixel.Position.Z <= zFromBuffer)
                    {
                        shaderContext.DepthBuffer.Set(y, x, pixel.Position.Z);
                        shaderContext.ColorBuffer.Set(y, x, pixelColor);
                    }
                }
                error2 += derror2;

                if (error2 > width)
                {
                    y += height > 0 ? 1 : -1;
                    error2 -= width * 2;
                }

                t += dt;
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