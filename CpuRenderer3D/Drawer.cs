using System.Numerics;

namespace CpuRenderer3D
{
    public static class Drawer
    {
        public static void DrawTriangle(RenderingContext shaderContext, IShaderProgram shaderProgram, FragmentInput f0, FragmentInput f1, FragmentInput f2)
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
        }


        public static void DrawLine(RenderingContext shaderContext, IShaderProgram shaderProgram, FragmentInput f0, FragmentInput f1)
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

        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
    }
}