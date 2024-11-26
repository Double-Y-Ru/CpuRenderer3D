using System;
using System.Numerics;

namespace CpuRenderer3D
{
    public static class Rasterizer
    {
        public delegate bool TestPixel<T>(int x, int y, T pixel);
        public delegate void SetPixel<T>(int x, int y, T pixel);

        public static void DrawTriangleBary<TFragmentData>(FragmentInput<TFragmentData> point0Clip, FragmentInput<TFragmentData> point1Clip, FragmentInput<TFragmentData> point2Clip, IInterpolator<TFragmentData> interpolator, Matrix4x4 clipScreen, TestPixel<FragmentInput<TFragmentData>> testPixel, SetPixel<FragmentInput<TFragmentData>> setPixel) where TFragmentData : struct
        {
            Vector4 point0Screen = Vector4.Transform(point0Clip.Position, clipScreen);
            Vector4 point1Screen = Vector4.Transform(point1Clip.Position, clipScreen);
            Vector4 point2Screen = Vector4.Transform(point2Clip.Position, clipScreen);

            Vector2 point0ScreenDivW = point0Screen.XYDivW();
            Vector2 point1ScreenDivW = point1Screen.XYDivW();
            Vector2 point2ScreenDivW = point2Screen.XYDivW();

            float triangleNormalZ = VectorExtenstions.Cross(
                point0ScreenDivW - point1ScreenDivW,
                point0ScreenDivW - point2ScreenDivW);

            if (triangleNormalZ < 0) return;

            GetBoundingBox(new Vector2[] { point0ScreenDivW, point1ScreenDivW, point2ScreenDivW }, new Vector2(2f * clipScreen.M41, 2f * clipScreen.M42), out Vector2 bboxmin, out Vector2 bboxmax);

            for (int y = (int)bboxmin.Y; y <= (int)bboxmax.Y; y++)
                for (int x = (int)bboxmin.X; x <= (int)bboxmax.X; x++)
                {
                    Vector2 pointScreen = new Vector2(x, y);
                    Vector3 pointBaryScreen = Barycentric(point0ScreenDivW, point1ScreenDivW, point2ScreenDivW, pointScreen);
                    Vector3 pointBaryClip = new Vector3(pointBaryScreen.X / point0Screen.W, pointBaryScreen.Y / point1Screen.W, pointBaryScreen.Z / point2Screen.W);
                    pointBaryClip = pointBaryClip / (pointBaryClip.X + pointBaryClip.Y + pointBaryClip.Z);

                    if (pointBaryScreen.X < 0 || pointBaryScreen.Y < 0 || pointBaryScreen.Z < 0) continue;

                    FragmentInput<TFragmentData> interpolatedFragInput = interpolator.InterpolateBary(point0Clip, point1Clip, point2Clip, pointBaryClip);

                    if (testPixel(x, y, interpolatedFragInput))
                        setPixel(x, y, interpolatedFragInput);
                }
        }

        public static void DrawTriangle<T>(Vector3 f0, Vector3 f1, Vector3 f2, T color, TestPixel<float> testDepth, SetPixel<float> setDepth, SetPixel<T> setColor)
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

        public static void DrawTriangle<TFragmentData>(FragmentInput<TFragmentData> point0Clip, FragmentInput<TFragmentData> point1Clip, FragmentInput<TFragmentData> point2Clip, IInterpolator<TFragmentData> interpolator, Matrix4x4 clipScreen, TestPixel<FragmentInput<TFragmentData>> testPixel, SetPixel<FragmentInput<TFragmentData>> setPixel) where TFragmentData : struct
        {
            Vector4 point0Screen = Vector4.Transform(point0Clip.Position, clipScreen);
            Vector4 point1Screen = Vector4.Transform(point1Clip.Position, clipScreen);
            Vector4 point2Screen = Vector4.Transform(point2Clip.Position, clipScreen);

            Vector2 point0ScreenDivW = point0Screen.XYDivW();
            Vector2 point1ScreenDivW = point1Screen.XYDivW();
            Vector2 point2ScreenDivW = point2Screen.XYDivW();

            float triangleNormalZ = VectorExtenstions.Cross(
                point0ScreenDivW - point1ScreenDivW,
                point0ScreenDivW - point2ScreenDivW);

            if (triangleNormalZ < 0) return;

            if (point0ScreenDivW.Y > point1ScreenDivW.Y)
            {
                Swap(ref point0Clip, ref point1Clip);
                Swap(ref point0ScreenDivW, ref point1ScreenDivW);
            }
            if (point0ScreenDivW.Y > point2ScreenDivW.Y)
            {
                Swap(ref point0Clip, ref point2Clip);
                Swap(ref point0ScreenDivW, ref point2ScreenDivW);
            }
            if (point1ScreenDivW.Y > point2ScreenDivW.Y)
            {
                Swap(ref point1Clip, ref point2Clip);
                Swap(ref point1ScreenDivW, ref point2ScreenDivW);
            }

            int lowerY = (int)MathF.Round(point0ScreenDivW.Y);
            int rightY = (int)MathF.Round(point1ScreenDivW.Y);
            int upperY = (int)MathF.Round(point2ScreenDivW.Y);

            float leftX = point0ScreenDivW.X;
            float leftXDelta = (point2ScreenDivW.X - point0ScreenDivW.X) / (upperY - lowerY + 1);
            FragmentInput<TFragmentData> leftFragInput = point0Clip;
            FragmentInput<TFragmentData> leftFragInputDelta = interpolator.Divide(interpolator.Subtract(point2Clip, point0Clip), upperY - lowerY + 1);

            float rightLowerX = point0ScreenDivW.X;
            float rightLowerXDelta = (point1ScreenDivW.X - point0ScreenDivW.X) / (rightY - lowerY + 1);
            FragmentInput<TFragmentData> rightLowerFragInput = point0Clip;
            FragmentInput<TFragmentData> rightLowerFragInputDelta = interpolator.Divide(interpolator.Subtract(point1Clip, point0Clip), rightY - lowerY + 1);

            float rightUpperX = point1ScreenDivW.X;
            float rightUpperXDelta = (point2ScreenDivW.X - point1ScreenDivW.X) / (upperY - rightY + 1);
            FragmentInput<TFragmentData> rightUpperFragInput = point1Clip;
            FragmentInput<TFragmentData> rightUpperFragInputDelta = interpolator.Divide(interpolator.Subtract(point2Clip, point1Clip), upperY - rightY + 1);

            for (int y = lowerY; y < rightY; ++y)
            {
                DrawHorizontalLine((int)MathF.Round(leftX), (int)MathF.Round(rightLowerX), leftFragInput, rightLowerFragInput, y);

                leftX += leftXDelta;
                rightLowerX += rightLowerXDelta;

                leftFragInput = interpolator.Add(leftFragInput, leftFragInputDelta);
                rightLowerFragInput = interpolator.Add(rightLowerFragInput, rightLowerFragInputDelta);
            }

            for (int y = rightY; y <= upperY; ++y)
            {
                DrawHorizontalLine((int)MathF.Round(leftX), (int)MathF.Round(rightUpperX), leftFragInput, rightUpperFragInput, y);

                leftX += leftXDelta;
                rightUpperX += rightUpperXDelta;

                leftFragInput = interpolator.Add(leftFragInput, leftFragInputDelta);
                rightUpperFragInput = interpolator.Add(rightUpperFragInput, rightUpperFragInputDelta);
            }

            void DrawHorizontalLine(int startX, int endX, FragmentInput<TFragmentData> lineStart, FragmentInput<TFragmentData> lineEnd, int y)
            {
                if (startX > endX)
                {
                    Swap(ref lineStart, ref lineEnd);
                    Swap(ref startX, ref endX);
                }

                FragmentInput<TFragmentData> fragInput = lineStart;
                FragmentInput<TFragmentData> fragInputDelta = interpolator.Divide(interpolator.Subtract(lineEnd, lineStart), endX - startX + 1);

                for (int x = startX; x <= endX; x++)
                {
                    if (testPixel(x, y, fragInput))
                        setPixel(x, y, fragInput);

                    fragInput = interpolator.Add(fragInput, fragInputDelta);
                }
            }
        }

        public static void DrawLine<T>(Vector3 f0, Vector3 f1, T color, TestPixel<float> testDepth, SetPixel<float> setDepth, SetPixel<T> setColor)
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
                    if (testDepth(x, y, z))
                    {
                        setDepth(x, y, z);
                        setColor(x, y, color);
                    }
                }
                else
                {
                    if (testDepth(y, x, z))
                    {
                        setDepth(y, x, z);
                        setColor(y, x, color);
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

        public static void DrawLine<TFragmentData>(FragmentInput<TFragmentData> f0, FragmentInput<TFragmentData> f1, IInterpolator<TFragmentData> interpolator, TestPixel<FragmentInput<TFragmentData>> testPixel, SetPixel<FragmentInput<TFragmentData>> setPixel) where TFragmentData : struct
        {
            bool isGentle = true;

            if (Math.Abs(f0.Position.X - f1.Position.X) < Math.Abs(f0.Position.Y - f1.Position.Y))
            {
                isGentle = false;

                Vector4 f0Pos = new Vector4(f0.Position.Y, f0.Position.X, f0.Position.Z, f0.Position.W);
                Vector4 f1Pos = new Vector4(f1.Position.Y, f1.Position.X, f1.Position.Z, f1.Position.W);

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
                    if (testPixel(x, y, fragInput))
                        setPixel(x, y, fragInput);
                }
                else
                {
                    if (testPixel(y, x, fragInput))
                        setPixel(y, x, fragInput);
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

        private static void GetBoundingBox(Vector2[] points, Vector2 clamp, out Vector2 bboxmin, out Vector2 bboxmax)
        {
            bboxmin = clamp;
            bboxmax = new Vector2(0f, 0f);

            for (int i = 0; i < points.Length; i++)
            {
                bboxmin.X = MathF.Max(0, MathF.Min(bboxmin.X, points[i].X));
                bboxmax.X = MathF.Min(clamp.X, MathF.Max(bboxmax.X, points[i].X));

                bboxmin.Y = MathF.Max(0, MathF.Min(bboxmin.Y, points[i].Y));
                bboxmax.Y = MathF.Min(clamp.Y, MathF.Max(bboxmax.Y, points[i].Y));
            }
        }

        private static Vector3 Barycentric(Vector2 point0, Vector2 point1, Vector2 point2, Vector2 P)
        {
            Vector3 u = Vector3.Cross(
                new Vector3(point2.X - point0.X,
                            point1.X - point0.X,
                            point0.X - P.X),
                new Vector3(point2.Y - point0.Y,
                            point1.Y - point0.Y,
                            point0.Y - P.Y));

            if (MathF.Abs(u.Z) < 1f) return new Vector3(-1f, 1f, 1f); // triangle is degenerate, in this case return smth with negative coordinates
            return new Vector3(1f - (u.X + u.Y) / u.Z, u.Y / u.Z, u.X / u.Z);
        }

        private static void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }
    }
}