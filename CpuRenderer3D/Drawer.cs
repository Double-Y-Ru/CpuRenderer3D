using System;
using System.Numerics;

namespace CpuRenderer3D
{
    public static class Drawer
    {
        public delegate bool TestDepth(int x, int y, float depth);
        public delegate void SetDepth(int x, int y, float depth);
        public delegate void SetColor<T>(int x, int y, T color);

        public static void DrawTriangle<T>(Vector3 f0, Vector3 f1, Vector3 f2, T color, TestDepth testDepth, SetDepth setDepth, SetColor<T> setColor)
        {
            if (f0.Y == f1.Y
             && f0.Y == f2.Y)
                return;

            if (f0.Y > f1.Y) Swap(ref f0, ref f1);
            if (f0.Y > f2.Y) Swap(ref f0, ref f2);
            if (f1.Y > f2.Y) Swap(ref f1, ref f2);

            int lowerY = (int)MathF.Round(f0.Y);
            int rightY = (int)MathF.Round(f1.Y);
            int upperY = (int)MathF.Round(f2.Y);

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

                int lineStartX = (int)MathF.Round(lineStart.X);
                int lineEndX = (int)MathF.Round(lineEnd.X);

                Vector3 point = lineStart;
                Vector3 pointDelta = (lineEnd - lineStart) / (lineEndX - lineStartX + 1);

                for (int x = lineStartX; x <= lineEndX; x++)
                {
                    if (testDepth(x, y, point.Z))
                    {
                        setDepth(x, y, point.Z);
                        setColor(x, y, color);
                    }

                    point = point + pointDelta;
                }
            }
        }

        public static void DrawTriangle<TFragmentData>(FragmentInput<TFragmentData> f0, FragmentInput<TFragmentData> f1, FragmentInput<TFragmentData> f2, IInterpolator<TFragmentData> interpolator, TestDepth testDepth, SetDepth setDepth, SetColor<FragmentInput<TFragmentData>> setColor) where TFragmentData : struct
        {
            if (f0.Position.Y == f1.Position.Y
             && f0.Position.Y == f2.Position.Y)
                return;

            if (f0.Position.Y > f1.Position.Y) Swap(ref f0, ref f1);
            if (f0.Position.Y > f2.Position.Y) Swap(ref f0, ref f2);
            if (f1.Position.Y > f2.Position.Y) Swap(ref f1, ref f2);

            int lowerY = (int)MathF.Round(f0.Position.Y);
            int rightY = (int)MathF.Round(f1.Position.Y);
            int upperY = (int)MathF.Round(f2.Position.Y);

            FragmentInput<TFragmentData> leftFragInput = f0;
            FragmentInput<TFragmentData> leftFragInputDelta = interpolator.Divide(interpolator.Subtract(f2, f0), upperY - lowerY + 1);

            FragmentInput<TFragmentData> rightLowerFragInput = f0;
            FragmentInput<TFragmentData> rightLowerFragInputDelta = interpolator.Divide(interpolator.Subtract(f1, f0), rightY - lowerY + 1);

            FragmentInput<TFragmentData> rightUpperFragInput = f1;
            FragmentInput<TFragmentData> rightUpperFragInputDelta = interpolator.Divide(interpolator.Subtract(f2, f1), upperY - rightY + 1);

            for (int y = lowerY; y < rightY; ++y)
            {
                DrawHorizontalLine(leftFragInput, rightLowerFragInput, y);

                leftFragInput = interpolator.Add(leftFragInput, leftFragInputDelta);
                rightLowerFragInput = interpolator.Add(rightLowerFragInput, rightLowerFragInputDelta);
            }

            for (int y = rightY; y <= upperY; ++y)
            {
                DrawHorizontalLine(leftFragInput, rightUpperFragInput, y);

                leftFragInput = interpolator.Add(leftFragInput, leftFragInputDelta);
                rightUpperFragInput = interpolator.Add(rightUpperFragInput, rightUpperFragInputDelta);
            }

            void DrawHorizontalLine(FragmentInput<TFragmentData> lineStart, FragmentInput<TFragmentData> lineEnd, int y)
            {
                if (lineStart.Position.X > lineEnd.Position.X)
                    Swap(ref lineStart, ref lineEnd);

                int lineStartX = (int)MathF.Round(lineStart.Position.X);
                int lineEndX = (int)MathF.Round(lineEnd.Position.X);

                FragmentInput<TFragmentData> fragInput = lineStart;
                FragmentInput<TFragmentData> fragInputDelta = interpolator.Divide(interpolator.Subtract(lineEnd, lineStart), lineEndX - lineStartX + 1);

                for (int x = lineStartX; x <= lineEndX; x++)
                {
                    if (testDepth(x, y, fragInput.Position.Z))
                    {
                        setDepth(x, y, fragInput.Position.Z);
                        setColor(x, y, fragInput);
                    }

                    fragInput = interpolator.Add(fragInput, fragInputDelta);
                }
            }
        }

        public static void DrawLine<T>(Vector3 f0, Vector3 f1, T color, Buffer<T> colorBuffer, Buffer<float> depthBuffer)
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

            int gentleLeftX = (int)MathF.Round(f0.X);
            int gentleLeftY = (int)MathF.Round(f0.Y);

            int gentleRightX = (int)MathF.Round(f1.X);
            int gentleRightY = (int)MathF.Round(f1.Y);

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
                    if (depthBuffer.TryGet(x, y, out float zFromBuffer) && z <= zFromBuffer)
                    {
                        depthBuffer.Set(x, y, z);
                        colorBuffer.Set(x, y, color);
                    }
                }
                else
                {
                    if (depthBuffer.TryGet(y, x, out float zFromBuffer) && z <= zFromBuffer)
                    {
                        depthBuffer.Set(y, x, z);
                        colorBuffer.Set(y, x, color);
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

        public static void DrawLine<TFragmentData>(FragmentInput<TFragmentData> f0, FragmentInput<TFragmentData> f1, IInterpolator<TFragmentData> interpolator, TestDepth testDepth, SetDepth setDepth, SetColor<FragmentInput<TFragmentData>> setColor) where TFragmentData : struct
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

            int gentleLeftX = (int)MathF.Round(f0.Position.X);
            int gentleLeftY = (int)MathF.Round(f0.Position.Y);

            int gentleRightX = (int)MathF.Round(f1.Position.X);
            int gentleRightY = (int)MathF.Round(f1.Position.Y);

            int width = gentleRightX - gentleLeftX;
            int height = gentleRightY - gentleLeftY;

            FragmentInput<TFragmentData> fragInput = f0;
            FragmentInput<TFragmentData> fragInputDelta = interpolator.Divide(interpolator.Subtract(f1, f0), width);

            int derror2 = Math.Abs(height) * 2;
            int error2 = 0;

            int y = gentleLeftY;
            for (int x = gentleLeftX; x <= gentleRightX; x++)
            {
                if (isGentle)
                {
                    if (testDepth(x, y, fragInput.Position.Z))
                    {
                        setDepth(x, y, fragInput.Position.Z);
                        setColor(x, y, fragInput);
                    }
                }
                else
                {
                    if (testDepth(y, x, fragInput.Position.Z))
                    {
                        setDepth(y, x, fragInput.Position.Z);
                        setColor(y, x, fragInput);
                    }
                }
                error2 += derror2;

                if (error2 > width)
                {
                    y += height > 0 ? 1 : -1;
                    error2 -= width * 2;
                }

                fragInput = interpolator.Add(fragInput, fragInputDelta);
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