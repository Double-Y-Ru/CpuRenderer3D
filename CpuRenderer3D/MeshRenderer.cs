using System.Numerics;

namespace CpuRenderer3D
{
    public class MeshRenderer : IRenderer
    {
        public void Render(Entity entity, RenderingContext shaderContext, ShaderProgram shaderProgram)
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
                    // новый
                    fstF.Position = Vector3.Transform(fstF.Position, shaderContext.ClipView);
                    sndF.Position = Vector3.Transform(sndF.Position, shaderContext.ClipView);
                    trdF.Position = Vector3.Transform(trdF.Position, shaderContext.ClipView);

                    DrawTriangle(shaderContext, shaderProgram, fstF, sndF, trdF);
                }
            }
        }

        /*public void Render(Entity entity, RenderingContext shaderContext, ShaderProgram shaderProgram)
        {
            VertexInput fstV, sndV, trdV;
            FragmentInput fstF, sndF, trdF;

            // новый
            shaderContext.SetModelWorld(entity.Transform.GetMatrix());

            // старый
            Transform camera = new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity);
            Matrix4x4.Invert(camera.GetMatrix(), out Matrix4x4 invertCamera);

            float aspect = (float)800 / 600;
            float nearPlane = 0.1f;
            float farPlane = 200;
            Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView((float)(0.5f * Math.PI), aspect, nearPlane, farPlane);

            Matrix4x4 projMview = invertCamera * proj;
            Matrix4x4 screenScale = new Matrix4x4(
                0.5f * 800, 0, 0, 0,
                0, 0.5f * 600, 0, 0,
                0, 0, 1, 0,
                0.5f * 800, 0.5f * 600, 0, 1);

            Matrix4x4 relativeTransform = entity.Transform.GetMatrix() * projMview;

            Mesh mesh = entity.Mesh;
            Triangle[] triangles = mesh.GetTriangles();

            for (int i = 0; i < triangles.Length; i++)
            {
                Triangle triangle = triangles[i];

                // новый
                fstV = new VertexInput(mesh.GetVertex(triangle.First), new Vector3(), new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                sndV = new VertexInput(mesh.GetVertex(triangle.Second), new Vector3(), new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());
                trdV = new VertexInput(mesh.GetVertex(triangle.Third), new Vector3(), new Vector4(0.2f, 0.2f, 0.2f, 1f), new Vector2(), new Vector2(), new Vector2(), new Vector2());

                fstF = shaderProgram.ComputeVertex(fstV, shaderContext);
                sndF = shaderProgram.ComputeVertex(sndV, shaderContext);
                trdF = shaderProgram.ComputeVertex(trdV, shaderContext);

                // старый
                FragmentInput fstF1 = new FragmentInput(mesh.GetVertex(triangle.First), fstV.Normal, fstV.Color, fstV.UV0, fstV.UV1, fstV.UV2, fstV.UV3);
                FragmentInput sndF1 = new FragmentInput(mesh.GetVertex(triangle.Second), fstV.Normal, fstV.Color, fstV.UV0, fstV.UV1, fstV.UV2, fstV.UV3);
                FragmentInput trdF1 = new FragmentInput(mesh.GetVertex(triangle.Third), fstV.Normal, fstV.Color, fstV.UV0, fstV.UV1, fstV.UV2, fstV.UV3);

                fstF1.Position = Vector4.Transform(new Vector4(fstF1.Position, 1f), relativeTransform).XYZDivW();
                sndF1.Position = Vector4.Transform(new Vector4(sndF1.Position, 1f), relativeTransform).XYZDivW();
                trdF1.Position = Vector4.Transform(new Vector4(trdF1.Position, 1f), relativeTransform).XYZDivW();

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
                    // новый
                    fstF.Position = Vector3.Transform(fstF.Position, shaderContext.ClipView);
                    sndF.Position = Vector3.Transform(sndF.Position, shaderContext.ClipView);
                    trdF.Position = Vector3.Transform(trdF.Position, shaderContext.ClipView);

                    // старый
                    fstF1.Position = Vector3.Transform(fstF1.Position, screenScale);
                    sndF1.Position = Vector3.Transform(sndF1.Position, screenScale);
                    trdF1.Position = Vector3.Transform(trdF1.Position, screenScale);

                    DrawTriangle(shaderContext, shaderProgram, fstF, sndF, trdF);
                    DrawTriangle(shaderContext, shaderProgram, fstF1, sndF1, trdF1);
                }
            }
        }*/

        private void DrawTriangle(RenderingContext shaderContext, ShaderProgram shaderProgram,
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
                if (lineWidth == 0) lineWidth = 1;
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

        private void DrawTriangle(RenderingContext shaderContext, Vector3 v0, Vector3 v1, Vector3 v2)
        {
            Vector3iif t0 = new Vector3iif(v0);
            Vector3iif t1 = new Vector3iif(v1);
            Vector3iif t2 = new Vector3iif(v2);

            if (t0.Y == t1.Y && t0.Y == t2.Y) return;
            if (t0.Y > t1.Y) Swap(ref t0, ref t1);
            if (t0.Y > t2.Y) Swap(ref t0, ref t2);
            if (t1.Y > t2.Y) Swap(ref t1, ref t2);

            int total_height = t2.Y - t0.Y;
            for (int y = 0; y < total_height; y++)
            {
                bool second_half = y > t1.Y - t0.Y || t1.Y == t0.Y;
                int segment_height = second_half ? t2.Y - t1.Y : t1.Y - t0.Y;
                float alpha = (float)y / total_height;
                float beta = (float)(y - (second_half ? t1.Y - t0.Y : 0)) / segment_height;
                Vector3iif A = t0 + (t2 - t0) * alpha;
                Vector3iif B = second_half ? t1 + (t2 - t1) * beta : t0 + (t1 - t0) * beta;
                if (A.X > B.X) Swap(ref A, ref B);
                float dz = (B.Z - A.Z) / (B.X - A.X);
                float z = A.Z;
                for (int x = A.X; x <= B.X; x++)
                {
                    var pixel = new Vector3iif(x, (t0.Y + y), z);
                    if (!shaderContext.ZBuffer.TryGet(x, y, out float zFromBuffer)) continue;
                    if (zFromBuffer < pixel.Z) continue;

                    shaderContext.ZBuffer.Set(x, y, pixel.Z);
                    shaderContext.ColorBuffer.Set(x, y, new Vector4(0.5f, 0.5f, 0.5f, 1f));
                    z += dz;
                }
            }

            void Swap(ref Vector3iif a, ref Vector3iif b)
            {
                Vector3iif c = a;
                a = b;
                b = c;
            }
        }
    }
}