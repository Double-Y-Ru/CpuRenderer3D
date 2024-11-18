using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class CpuRendererLegacy : ICpuRenderer
    {
        public void Render(IReadOnlyList<Entity> entities, Camera camera, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
        {
            Matrix4x4 worldProjection = camera.GetWorldViewMatrix() * camera.GetViewProjectionMatrix();

            Matrix4x4 projectionClip = Util.CreateProjectionClip(colorBuffer.Width, colorBuffer.Height);

            Vector4 triangleColor = new Vector4(0.2f, 0.2f, 0.2f, 1f);
            Vector4 lineColor = new Vector4(0.5f, 0.5f, 0.5f, 1f);

            foreach (Entity entity in entities)
            {
                Matrix4x4 modelWorld = entity.Transform.GetMatrix();
                Matrix4x4 modelProjection = modelWorld * worldProjection;

                Mesh mesh = entity.Mesh;
                Triangle[] triangles = mesh.GetTriangles();

                for (int i = 0; i < triangles.Length; i++)
                {
                    Vector3 triangleV1P = Vector4.Transform(mesh.GetVertex(triangles[i].V0), modelProjection).XYZDivW();
                    Vector3 triangleV2P = Vector4.Transform(mesh.GetVertex(triangles[i].V1), modelProjection).XYZDivW();
                    Vector3 triangleV3P = Vector4.Transform(mesh.GetVertex(triangles[i].V2), modelProjection).XYZDivW();

                    Vector3 triangleNormalP = Vector3.Cross(
                        triangleV1P - triangleV2P,
                        triangleV1P - triangleV3P);

                    if (Vector3.Dot(triangleNormalP, Vector3.UnitZ) > 0)
                    {
                        if (-1f < triangleV1P.X && triangleV1P.X < 1f
                         && -1f < triangleV1P.Y && triangleV1P.Y < 1f
                         && -1f < triangleV1P.Z && triangleV1P.Z < 1f
                         && -1f < triangleV2P.X && triangleV2P.X < 1f
                         && -1f < triangleV2P.Y && triangleV2P.Y < 1f
                         && -1f < triangleV2P.Z && triangleV2P.Z < 1f
                         && -1f < triangleV3P.X && triangleV3P.X < 1f
                         && -1f < triangleV3P.Y && triangleV3P.Y < 1f
                         && -1f < triangleV3P.Z && triangleV3P.Z < 1f
                         )
                        {
                            Vector3 triangleV1S = Vector3.Transform(triangleV1P, projectionClip);
                            Vector3 triangleV2S = Vector3.Transform(triangleV2P, projectionClip);
                            Vector3 triangleV3S = Vector3.Transform(triangleV3P, projectionClip);

                            DrawTriangle(triangleV1S, triangleV2S, triangleV3S, triangleColor, colorBuffer, depthBuffer);

                            DrawLine(triangleV1S - Vector3.UnitZ * 0.0001f, triangleV2S - Vector3.UnitZ * 0.0001f, lineColor, colorBuffer, depthBuffer);
                            DrawLine(triangleV1S - Vector3.UnitZ * 0.0001f, triangleV3S - Vector3.UnitZ * 0.0001f, lineColor, colorBuffer, depthBuffer);
                            DrawLine(triangleV2S - Vector3.UnitZ * 0.0001f, triangleV3S - Vector3.UnitZ * 0.0001f, lineColor, colorBuffer, depthBuffer);
                        }
                    }
                }
            }
        }

        private void DrawLine(Vector3 v0, Vector3 v1, Vector4 color, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
        {
            DrawLine(new Vector3iif(v0), new Vector3iif(v1), color, colorBuffer, depthBuffer);
        }

        private void DrawLine(Vector3iif v0, Vector3iif v1, Vector4 color, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
        {
            bool steep = false;
            if (Math.Abs(v0.X - v1.X) < Math.Abs(v0.Y - v1.Y))
            {
                steep = true;
                v0 = new Vector3iif(v0.Y, v0.X, v0.Z);
                v1 = new Vector3iif(v1.Y, v1.X, v1.Z);
            }

            if (v0.X > v1.X)
            {
                Swap(ref v0, ref v1);
            }
            int dx = v1.X - v0.X;
            int dy = v1.Y - v0.Y;
            float dz = (v1.Z - v0.Z) / (v1.X - v0.X);

            int derror2 = Math.Abs(dy) * 2;
            int error2 = 0;

            int y = v0.Y;
            float z = v0.Z;
            for (int x = v0.X; x <= v1.X; x++)
            {
                if (steep)
                {
                    DrawPixel(new Vector3iif(y, x, z), color, colorBuffer, depthBuffer);
                }
                else
                {
                    DrawPixel(new Vector3iif(x, y, z), color, colorBuffer, depthBuffer);
                }
                error2 += derror2;

                if (error2 > dx)
                {
                    y += v1.Y > v0.Y ? 1 : -1;
                    error2 -= dx * 2;
                }

                z += dz;
            }

            void Swap(ref Vector3iif a, ref Vector3iif b)
            {
                Vector3iif c = a;
                a = b;
                b = c;
            }
        }

        private void DrawTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector4 color, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
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
                    DrawPixel(new Vector3iif(x, t0.Y + y, z), color, colorBuffer, depthBuffer);
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

        private void DrawPixel(Vector3iif v, Vector4 color, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
        {
            if (depthBuffer.TryGet(v.X, v.Y, out float zFromBuffer) && v.Z <= zFromBuffer)
            {
                depthBuffer.Set(v.X, v.Y, v.Z);
                colorBuffer.Set(v.X, v.Y, color);
            }
        }
    }
}