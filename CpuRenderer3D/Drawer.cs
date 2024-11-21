using System.Numerics;

namespace CpuRenderer3D
{
    public static class Drawer
    {
        public static void DrawTriangle(RenderingContext context, Vector3 f0, Vector3 f1, Vector3 f2, Vector4 color)
        {
            if (f0.Y == f1.Y
             && f0.Y == f2.Y)
                return;

            if (f0.Y > f1.Y) Swap(ref f0, ref f1);
            if (f0.Y > f2.Y) Swap(ref f0, ref f2);
            if (f1.Y > f2.Y) Swap(ref f1, ref f2);

            int lowerY = (int)float.Round(f0.Y);
            int rightY = (int)float.Round(f1.Y);
            int upperY = (int)float.Round(f2.Y);

            Vector3 leftPoint = f0;
            Vector3 leftPointDelta = (f2 - f0) / (upperY - lowerY + 1);

            Vector3 rightLowerPoint = f0;
            Vector3 rightLowerPointDelta = (f1 - f0) / (rightY - lowerY + 1);

            Vector3 rightUpperPoint = f1;
            Vector3 rightUpperPointDelta = (f2 - f1) / (upperY - rightY + 1);

            for (int y = lowerY; y < rightY; ++y)
            {
                DrawHorizontalLine(leftPoint, rightLowerPoint, y);

                leftPoint = leftPoint + leftPointDelta;
                rightLowerPoint = rightLowerPoint + rightLowerPointDelta;
            }

            for (int y = rightY; y <= upperY; ++y)
            {
                DrawHorizontalLine(leftPoint, rightUpperPoint, y);

                leftPoint = leftPoint + leftPointDelta;
                rightUpperPoint = rightUpperPoint + rightUpperPointDelta;
            }

            void DrawHorizontalLine(Vector3 lineStart, Vector3 lineEnd, int y)
            {
                if (lineStart.X > lineEnd.X)
                    Swap(ref lineStart, ref lineEnd);

                int lineStartX = (int)float.Round(lineStart.X);
                int lineEndX = (int)float.Round(lineEnd.X);

                Vector3 point = lineStart;
                Vector3 pointDelta = (lineEnd - lineStart) / (lineEndX - lineStartX + 1);

                for (int x = lineStartX; x <= lineEndX; x++)
                {
                    if (context.DepthBuffer.TryGet(x, y, out float zFromBuffer) && point.Z <= zFromBuffer)
                    {
                        context.DepthBuffer.Set(x, y, point.Z);
                        context.ColorBuffer.Set(x, y, color);
                    }

                    point = point + pointDelta;
                }
            }
        }

        public static void DrawTriangle<TFragmentData>(RenderingContext context, FragmentInput<TFragmentData> f0, FragmentInput<TFragmentData> f1, FragmentInput<TFragmentData> f2, IShaderProgram<TFragmentData> shaderProgram) where TFragmentData : struct
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

            FragmentInput<TFragmentData> leftFragInput = f0;
            FragmentInput<TFragmentData> leftFragInputDelta = shaderProgram.Divide(shaderProgram.Subtract(f2, f0), upperY - lowerY + 1);

            FragmentInput<TFragmentData> rightLowerFragInput = f0;
            FragmentInput<TFragmentData> rightLowerFragInputDelta = shaderProgram.Divide(shaderProgram.Subtract(f1, f0), rightY - lowerY + 1);

            FragmentInput<TFragmentData> rightUpperFragInput = f1;
            FragmentInput<TFragmentData> rightUpperFragInputDelta = shaderProgram.Divide(shaderProgram.Subtract(f2, f1), upperY - rightY + 1);

            for (int y = lowerY; y < rightY; ++y)
            {
                DrawHorizontalLine(leftFragInput, rightLowerFragInput, y);

                leftFragInput = shaderProgram.Add(leftFragInput, leftFragInputDelta);
                rightLowerFragInput = shaderProgram.Add(rightLowerFragInput, rightLowerFragInputDelta);
            }

            for (int y = rightY; y <= upperY; ++y)
            {
                DrawHorizontalLine(leftFragInput, rightUpperFragInput, y);

                leftFragInput = shaderProgram.Add(leftFragInput, leftFragInputDelta);
                rightUpperFragInput = shaderProgram.Add(rightUpperFragInput, rightUpperFragInputDelta);
            }

            void DrawHorizontalLine(FragmentInput<TFragmentData> lineStart, FragmentInput<TFragmentData> lineEnd, int y)
            {
                if (lineStart.Position.X > lineEnd.Position.X)
                    Swap(ref lineStart, ref lineEnd);

                int lineStartX = (int)float.Round(lineStart.Position.X);
                int lineEndX = (int)float.Round(lineEnd.Position.X);

                FragmentInput<TFragmentData> fragInput = lineStart;
                FragmentInput<TFragmentData> fragInputDelta = shaderProgram.Divide(shaderProgram.Subtract(lineEnd, lineStart), lineEndX - lineStartX + 1);

                for (int x = lineStartX; x <= lineEndX; x++)
                {
                    Vector4 pixelColor = shaderProgram.ComputeColor(fragInput, context);

                    if (context.DepthBuffer.TryGet(x, y, out float zFromBuffer) && fragInput.Position.Z <= zFromBuffer)
                    {
                        context.DepthBuffer.Set(x, y, fragInput.Position.Z);
                        context.ColorBuffer.Set(x, y, pixelColor);
                    }

                    fragInput = shaderProgram.Add(fragInput, fragInputDelta);
                }
            }
        }

        public static void DrawLine(RenderingContext context, Vector3 f0, Vector3 f1, Vector4 color)
        {
            bool isGentle = true;

            if (Math.Abs(f0.X - f1.X) < Math.Abs(f0.Y - f1.Y))
            {
                isGentle = false;

                f0 = new Vector3(f0.Y, f0.X, f0.Z);
                f1 = new Vector3(f1.Y, f1.X, f1.Z);
            }

            if (f0.X > f1.X)
            {
                Swap(ref f0, ref f1);
            }

            int gentleLeftX = (int)float.Round(f0.X);
            int gentleLeftY = (int)float.Round(f0.Y);

            int gentleRightX = (int)float.Round(f1.X);
            int gentleRightY = (int)float.Round(f1.Y);

            int width = gentleRightX - gentleLeftX;
            int height = gentleRightY - gentleLeftY;

            float z = f0.Z;
            float zDelta = (f1.Z - f0.Z) / width;

            int derror2 = Math.Abs(height) * 2;
            int error2 = 0;

            int y = gentleLeftY;
            for (int x = gentleLeftX; x <= gentleRightX; x++)
            {
                if (isGentle)
                {
                    if (context.DepthBuffer.TryGet(x, y, out float zFromBuffer) && z <= zFromBuffer)
                    {
                        context.DepthBuffer.Set(x, y, z);
                        context.ColorBuffer.Set(x, y, color);
                    }
                }
                else
                {
                    if (context.DepthBuffer.TryGet(y, x, out float zFromBuffer) && z <= zFromBuffer)
                    {
                        context.DepthBuffer.Set(y, x, z);
                        context.ColorBuffer.Set(y, x, color);
                    }
                }
                error2 += derror2;

                if (error2 > width)
                {
                    y += height > 0 ? 1 : -1;
                    error2 -= width * 2;
                }

                z += zDelta;
            }
        }

        public static void DrawLine<TFragmentData>(RenderingContext context, FragmentInput<TFragmentData> f0, FragmentInput<TFragmentData> f1, IShaderProgram<TFragmentData> shaderProgram) where TFragmentData : struct
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

            FragmentInput<TFragmentData> fragInput = f0;
            FragmentInput<TFragmentData> fragInputDelta = shaderProgram.Divide(shaderProgram.Subtract(f1, f0), width);

            int derror2 = Math.Abs(height) * 2;
            int error2 = 0;

            int y = gentleLeftY;
            for (int x = gentleLeftX; x <= gentleRightX; x++)
            {
                Vector4 pixelColor = shaderProgram.ComputeColor(fragInput, context);

                if (isGentle)
                {
                    if (context.DepthBuffer.TryGet(x, y, out float zFromBuffer) && fragInput.Position.Z <= zFromBuffer)
                    {
                        context.DepthBuffer.Set(x, y, fragInput.Position.Z);
                        context.ColorBuffer.Set(x, y, pixelColor);
                    }
                }
                else
                {
                    if (context.DepthBuffer.TryGet(y, x, out float zFromBuffer) && fragInput.Position.Z <= zFromBuffer)
                    {
                        context.DepthBuffer.Set(y, x, fragInput.Position.Z);
                        context.ColorBuffer.Set(y, x, pixelColor);
                    }
                }
                error2 += derror2;

                if (error2 > width)
                {
                    y += height > 0 ? 1 : -1;
                    error2 -= width * 2;
                }

                fragInput = shaderProgram.Add(fragInput, fragInputDelta);
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