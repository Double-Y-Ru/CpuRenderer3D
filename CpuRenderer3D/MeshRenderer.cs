using System.Numerics;

namespace CpuRenderer3D
{
    public class MeshRenderer : IRenderer
    {
        public void Render(Entity entity, RenderingContext shaderContext, IShaderProgram shaderProgram)
        {
            VertexInput fstV, sndV, trdV;
            FragmentInput fstF, sndF, trdF;

            shaderContext.SetModelWorld(entity.Transform.GetMatrix());

            Mesh mesh = entity.Mesh;
            Triangle[] triangles = mesh.GetTriangles();

            for (int i = 0; i < triangles.Length; i++)
            {
                Triangle triangle = triangles[i];

                fstV = new VertexInput(mesh.GetVertex(triangle.First), new Vector3(), new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                sndV = new VertexInput(mesh.GetVertex(triangle.Second), new Vector3(), new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                trdV = new VertexInput(mesh.GetVertex(triangle.Third), new Vector3(), new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());

                fstF = shaderProgram.ComputeVertex(fstV, shaderContext);
                sndF = shaderProgram.ComputeVertex(sndV, shaderContext);
                trdF = shaderProgram.ComputeVertex(trdV, shaderContext);

                Vector3 triangleNormalP = Vector3.Cross(
                    fstF.Position - sndF.Position,
                    fstF.Position - trdF.Position);

                if (Vector3.Dot(triangleNormalP, Vector3.UnitZ) < 0) continue;

                if (-1f < fstF.Position.X && fstF.Position.X < 1f
                 && -1f < fstF.Position.Y && fstF.Position.Y < 1f
                 && -1f < fstF.Position.Z && fstF.Position.Z < 1f
                 && -1f < sndF.Position.X && sndF.Position.X < 1f
                 && -1f < sndF.Position.Y && sndF.Position.Y < 1f
                 && -1f < sndF.Position.Z && sndF.Position.Z < 1f
                 && -1f < trdF.Position.X && trdF.Position.X < 1f
                 && -1f < trdF.Position.Y && trdF.Position.Y < 1f
                 && -1f < trdF.Position.Z && trdF.Position.Z < 1f
                 )
                {
                    fstF.Position = Vector3.Transform(fstF.Position, shaderContext.ProjectionClip);
                    sndF.Position = Vector3.Transform(sndF.Position, shaderContext.ProjectionClip);
                    trdF.Position = Vector3.Transform(trdF.Position, shaderContext.ProjectionClip);

                    DrawTriangle(shaderContext, shaderProgram, fstF, sndF, trdF);
                }
            }
        }

        private void DrawTriangle(RenderingContext shaderContext, IShaderProgram shaderProgram,
            FragmentInput f0, FragmentInput f1, FragmentInput f2)
        {
            f0.RoundXYPos();
            f1.RoundXYPos();
            f2.RoundXYPos();

            if (f0.Position.Y == f1.Position.Y
             && f0.Position.Y == f2.Position.Y)
                return;

            if (f0.Position.Y > f1.Position.Y) Swap(ref f0, ref f1);
            if (f0.Position.Y > f2.Position.Y) Swap(ref f0, ref f2);
            if (f1.Position.Y > f2.Position.Y) Swap(ref f1, ref f2);

            FragmentInput LeftPoint, RightPoint, pixel;
            bool isSecondSegment;
            int segmentHeight, lineWidth;
            float alpha, beta, offset;
            Vector4 pixelColor;
            int totalHeight = (int)f2.Position.Y - (int)f0.Position.Y;
            int secondSegmentY = (int)f1.Position.Y - (int)f0.Position.Y;

            for (int y = 0; y < totalHeight; y++)
            {
                isSecondSegment = y > f1.Position.Y - f0.Position.Y
                                   || f1.Position.Y == f0.Position.Y;
                segmentHeight = isSecondSegment
                    ? (int)f2.Position.Y - (int)f1.Position.Y
                    : (int)f1.Position.Y - (int)f0.Position.Y;

                offset = isSecondSegment ? secondSegmentY : 0;
                alpha = (float)y / totalHeight;
                beta = (float)((y - offset) / segmentHeight);

                LeftPoint = FragmentInput.Interpolate(f0, f2, alpha);
                RightPoint = isSecondSegment
                    ? FragmentInput.Interpolate(f1, f2, beta)
                    : FragmentInput.Interpolate(f0, f1, beta);

                LeftPoint.RoundXYPos();
                RightPoint.RoundXYPos();

                if (LeftPoint.Position.X > RightPoint.Position.X)
                    Swap(ref LeftPoint, ref RightPoint);

                lineWidth = (int)RightPoint.Position.X - (int)LeftPoint.Position.X + 1;
                for (int x = (int)LeftPoint.Position.X; x <= (int)RightPoint.Position.X; x++)
                {
                    pixel = FragmentInput.Interpolate(LeftPoint, RightPoint, (float)x / lineWidth);
                    if (!shaderContext.ZBuffer.TryGet(x, y, out float z)) continue;
                    if (z < pixel.Position.Z) continue;

                    shaderContext.ZBuffer.Set(x, y, pixel.Position.Z);
                    pixelColor = shaderProgram.ComputeColor(pixel, shaderContext);
                    shaderContext.ColorBuffer.Set(x, y, pixelColor);
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