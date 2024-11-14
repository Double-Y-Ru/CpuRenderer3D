using System.Drawing;
using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class CpuRendererLegacy
    {
        public void Render(Transform camera, IReadOnlyList<Entity> entities, Bytemap bytemap)
        {
            bytemap.Clear();

            Matrix4x4.Invert(camera.GetMatrix(), out Matrix4x4 invertCamera);

            float aspect = (float)bytemap.Width / bytemap.Height;
            float nearPlane = 0.1f;
            float farPlane = 200;
            //Matrix4x4 proj = Matrix4x4.CreateOrthographicOffCenter(left: -10 * aspect, right: 10 * aspect, 
            //    bottom: -10, top: 10, nearPlane, farPlane);
            Matrix4x4 proj = Matrix4x4.CreatePerspectiveFieldOfView((float)(0.5 * Math.PI), aspect, nearPlane, farPlane);

            Matrix4x4 projMview = invertCamera * proj;

            Matrix4x4 screenScale = new Matrix4x4(
                0.5f * bytemap.Width, 0, 0, 0,
                0, 0.5f * bytemap.Height, 0, 0,
                0, 0, 1, 0,
                0.5f * bytemap.Width, 0.5f * bytemap.Height, 0, 1);

            foreach (Entity entity in entities)
            {
                Matrix4x4 relativeTransform = entity.Transform.GetMatrix() * projMview;

                Mesh mesh = entity.Mesh;
                Triangle[] triangles = mesh.GetTriangles();

                for (int i = 0; i < triangles.Length; i++)
                {
                    Triangle triangle = triangles[i];
                    Vector4 triangleV1 = new Vector4(mesh.GetVertex(triangle.First), 1f);
                    Vector4 triangleV2 = new Vector4(mesh.GetVertex(triangle.Second), 1f);
                    Vector4 triangleV3 = new Vector4(mesh.GetVertex(triangle.Third), 1f);

                    Vector3 triangleV1P = Vector4.Transform(triangleV1, relativeTransform).XYZDivW();
                    Vector3 triangleV2P = Vector4.Transform(triangleV2, relativeTransform).XYZDivW();
                    Vector3 triangleV3P = Vector4.Transform(triangleV3, relativeTransform).XYZDivW();

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
                            Vector3 triangleV1S = Vector3.Transform(triangleV1P, screenScale);
                            Vector3 triangleV2S = Vector3.Transform(triangleV2P, screenScale);
                            Vector3 triangleV3S = Vector3.Transform(triangleV3P, screenScale);

                            /*Console.WriteLine("old");
                            Console.WriteLine("new");
                            Console.WriteLine(invertCamera);
                            Console.WriteLine(renderingContext.WorldView);
                            Console.WriteLine(proj);
                            Console.WriteLine(renderingContext.ViewProjection);
                            Console.WriteLine(projMview);
                            Console.WriteLine(renderingContext.WorldProjection);
                            Console.WriteLine(relativeTransform);
                            Console.WriteLine(renderingContext.ModelProjection);
                            Console.WriteLine(screenScale);
                            Console.WriteLine(renderingContext.ClipView);
                            Console.WriteLine();*/

                            DrawTriangle(bytemap, triangleV1S, triangleV2S, triangleV3S, Color.FromArgb(45, 45, 45));

                            DrawLine(bytemap, triangleV1S - Vector3.UnitZ * 0.0001f, triangleV2S - Vector3.UnitZ * 0.0001f, Color.DarkGray);
                            DrawLine(bytemap, triangleV1S - Vector3.UnitZ * 0.0001f, triangleV3S - Vector3.UnitZ * 0.0001f, Color.DarkGray);
                            DrawLine(bytemap, triangleV2S - Vector3.UnitZ * 0.0001f, triangleV3S - Vector3.UnitZ * 0.0001f, Color.DarkGray);
                        }
                    }
                }
            }
        }

        private void DrawPixel(Bytemap bytemap, Vector3iif v, Color color)
        {
            bytemap.SetPixel(v, color);
        }

        private void DrawLine(Bytemap bytemap, Vector3 v0, Vector3 v1, Color color)
        {
            DrawLine(bytemap, new Vector3iif(v0), new Vector3iif(v1), color);
        }

        private void DrawLine(Bytemap bytemap, Vector3iif v0, Vector3iif v1, Color color)
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
                    DrawPixel(bytemap, new Vector3iif(y, x, z), color);
                }
                else
                {
                    DrawPixel(bytemap, new Vector3iif(x, y, z), color);
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

        private void DrawTriangle(Bytemap bytemap, Vector3 v0, Vector3 v1, Vector3 v2, Color color)
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
                    DrawPixel(bytemap, new Vector3iif(x, t0.Y + y, z), color);
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