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

            FragmentInput leftFragInput = f0;
            FragmentInput leftFragInputDelta = (f2 - f0) / (upperY - lowerY + 1);

            FragmentInput rightLowerFragInput = f0;
            FragmentInput rightLowerFragInputDelta = (f1 - f0) / (rightY - lowerY + 1);

            FragmentInput rightUpperFragInput = f1;
            FragmentInput rightUpperFragInputDelta = (f2 - f1) / (upperY - rightY + 1);

            for (int y = lowerY; y < rightY; ++y)
            {
                DrawHorizontalLine(leftFragInput, rightLowerFragInput, y);

                leftFragInput += leftFragInputDelta;
                rightLowerFragInput += rightLowerFragInputDelta;
            }

            for (int y = rightY; y <= upperY; ++y)
            {
                DrawHorizontalLine(leftFragInput, rightUpperFragInput, y);

                leftFragInput += leftFragInputDelta;
                rightUpperFragInput += rightUpperFragInputDelta;
            }

            void DrawHorizontalLine(FragmentInput lineStart, FragmentInput lineEnd, int y)
            {
                if (lineStart.Position.X > lineEnd.Position.X)
                    Swap(ref lineStart, ref lineEnd);

                int lineStartX = (int)float.Round(lineStart.Position.X);
                int lineEndX = (int)float.Round(lineEnd.Position.X);

                FragmentInput fragInput = lineStart;
                FragmentInput deltaFragInput = (lineEnd - lineStart) / (lineEndX - lineStartX + 1);

                for (int x = lineStartX; x <= lineEndX; x++)
                {
                    Vector4 pixelColor = shaderProgram.ComputeColor(fragInput, shaderContext);

                    if (shaderContext.DepthBuffer.TryGet(x, y, out float zFromBuffer) && fragInput.Position.Z <= zFromBuffer)
                    {
                        shaderContext.DepthBuffer.Set(x, y, fragInput.Position.Z);
                        shaderContext.ColorBuffer.Set(x, y, pixelColor);
                    }

                    fragInput += deltaFragInput;
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

            FragmentInput fragInput = f0;
            FragmentInput fragInputDelta = (f1 - f0) / width;

            int derror2 = Math.Abs(height) * 2;
            int error2 = 0;

            int y = gentleLeftY;
            for (int x = gentleLeftX; x <= gentleRightX; x++)
            {
                Vector4 pixelColor = shaderProgram.ComputeColor(fragInput, shaderContext);

                if (isGentle)
                {
                    if (shaderContext.DepthBuffer.TryGet(x, y, out float zFromBuffer) && fragInput.Position.Z <= zFromBuffer)
                    {
                        shaderContext.DepthBuffer.Set(x, y, fragInput.Position.Z);
                        shaderContext.ColorBuffer.Set(x, y, pixelColor);
                    }
                }
                else
                {
                    if (shaderContext.DepthBuffer.TryGet(y, x, out float zFromBuffer) && fragInput.Position.Z <= zFromBuffer)
                    {
                        shaderContext.DepthBuffer.Set(y, x, fragInput.Position.Z);
                        shaderContext.ColorBuffer.Set(y, x, pixelColor);
                    }
                }
                error2 += derror2;

                if (error2 > width)
                {
                    y += height > 0 ? 1 : -1;
                    error2 -= width * 2;
                }

                fragInput += fragInputDelta;
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