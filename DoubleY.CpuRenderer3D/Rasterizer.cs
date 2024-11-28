using System;
using System.Numerics;

namespace DoubleY.CpuRenderer3D
{
    public static class Rasterizer
    {
        public delegate bool TestPixel<T>(int x, int y, T pixel);
        public delegate void SetPixel<T>(int x, int y, T pixel);

        public static void DrawTriangle<TInterpolatedData>(Vector4 point0Screen, TInterpolatedData point0Data, Vector4 point1Screen, TInterpolatedData point1Data, Vector4 point2Screen, TInterpolatedData point2Data, IInterpolator<TInterpolatedData> interpolator, Bounds3 screenBounds, TestPixel<float> testDepth, SetPixel<float> setDepth, SetPixel<TInterpolatedData> setPixel) where TInterpolatedData : struct
        {
            Vector3 point0ScreenDivW = point0Screen.XYZDivW();
            Vector3 point1ScreenDivW = point1Screen.XYZDivW();
            Vector3 point2ScreenDivW = point2Screen.XYZDivW();

            Vector2 point0ScreenDivWxy = point0ScreenDivW.XY();
            Vector2 point1ScreenDivWxy = point1ScreenDivW.XY();
            Vector2 point2ScreenDivWxy = point2ScreenDivW.XY();

            float triangleNormalZ = VectorExtenstions.Cross(
                point0ScreenDivWxy - point1ScreenDivWxy,
                point0ScreenDivWxy - point2ScreenDivWxy);

            if (triangleNormalZ < 0) return;

            if (!screenBounds.Contains(point0ScreenDivW)
             || !screenBounds.Contains(point1ScreenDivW)
             || !screenBounds.Contains(point2ScreenDivW))
                return;

            if (point0ScreenDivWxy.Y > point1ScreenDivWxy.Y)
            {
                Swap(ref point0Data, ref point1Data);
                Swap(ref point0Screen, ref point1Screen);
                Swap(ref point0ScreenDivWxy, ref point1ScreenDivWxy);
            }
            if (point0ScreenDivWxy.Y > point2ScreenDivWxy.Y)
            {
                Swap(ref point0Data, ref point2Data);
                Swap(ref point0Screen, ref point2Screen);
                Swap(ref point0ScreenDivWxy, ref point2ScreenDivWxy);
            }
            if (point1ScreenDivWxy.Y > point2ScreenDivWxy.Y)
            {
                Swap(ref point1Data, ref point2Data);
                Swap(ref point1Screen, ref point2Screen);
                Swap(ref point1ScreenDivWxy, ref point2ScreenDivWxy);
            }

            int lowerY = (int)MathF.Round(point0ScreenDivWxy.Y);
            int rightY = (int)MathF.Round(point1ScreenDivWxy.Y);
            int upperY = (int)MathF.Round(point2ScreenDivWxy.Y);

            float leftX = point0ScreenDivWxy.X;
            float leftXDelta = (point2ScreenDivWxy.X - point0ScreenDivWxy.X) / (upperY - lowerY + 1);

            float rightLowerX = point0ScreenDivWxy.X;
            float rightLowerXDelta = (point1ScreenDivWxy.X - point0ScreenDivWxy.X) / (rightY - lowerY + 1);

            float rightUpperX = point1ScreenDivWxy.X;
            float rightUpperXDelta = (point2ScreenDivWxy.X - point1ScreenDivWxy.X) / (upperY - rightY + 1);

            for (int y = lowerY; y < rightY; ++y)
            {
                DrawHorizontalLine((int)MathF.Round(leftX), (int)MathF.Round(rightLowerX), y);

                leftX += leftXDelta;
                rightLowerX += rightLowerXDelta;
            }

            for (int y = rightY; y <= upperY; ++y)
            {
                DrawHorizontalLine((int)MathF.Round(leftX), (int)MathF.Round(rightUpperX), y);

                leftX += leftXDelta;
                rightUpperX += rightUpperXDelta;
            }

            void DrawHorizontalLine(int startX, int endX, int y)
            {
                if (startX > endX) Swap(ref startX, ref endX);

                for (int x = startX; x <= endX; x++)
                {
                    Vector2 pointScreen = new Vector2(x, y);

                    Vector3 pointBaryScreen = Barycentric(point0ScreenDivWxy, point1ScreenDivWxy, point2ScreenDivWxy, pointScreen);
                    Vector3 pointBaryClip = new Vector3(pointBaryScreen.X / point0Screen.W, pointBaryScreen.Y / point1Screen.W, pointBaryScreen.Z / point2Screen.W);
                    pointBaryClip = pointBaryClip / (pointBaryClip.X + pointBaryClip.Y + pointBaryClip.Z);

                    TInterpolatedData interpolatedFragInput = interpolator.InterpolateBary(point0Data, point1Data, point2Data, pointBaryClip);

                    float screenDepth = Vector3.Dot(new Vector3(point0Screen.Z / point0Screen.W, point1Screen.Z / point1Screen.W, point2Screen.Z / point2Screen.W), pointBaryScreen);

                    if (testDepth(x, y, screenDepth))
                    {
                        setDepth(x, y, screenDepth);
                        setPixel(x, y, interpolatedFragInput);
                    }
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

        public static void DrawLine<TInterpolatedData>(Vector4 point0Screen, Vector4 point1Screen, TInterpolatedData pixelData, Bounds3 screenBounds, TestPixel<float> testDepth, SetPixel<float> setDepth, SetPixel<TInterpolatedData> setPixel) where TInterpolatedData : struct
        {
            Vector3 point0ScreenDivW = point0Screen.XYZDivW();
            Vector3 point1ScreenDivW = point1Screen.XYZDivW();

            Vector2 point0ScreenDivWxy = point0ScreenDivW.XY();
            Vector2 point1ScreenDivWxy = point1ScreenDivW.XY();

            if (!screenBounds.Contains(point0ScreenDivW)
             || !screenBounds.Contains(point1ScreenDivW))
                return;

            bool isGentle = true;

            if (Math.Abs(point0ScreenDivW.X - point1ScreenDivW.X) < Math.Abs(point0ScreenDivW.Y - point1ScreenDivW.Y))
            {
                isGentle = false;

                point0ScreenDivW = new Vector3(point0ScreenDivW.Y, point0ScreenDivW.X, point0ScreenDivW.Z);
                point1ScreenDivW = new Vector3(point1ScreenDivW.Y, point1ScreenDivW.X, point1ScreenDivW.Z);
            }

            if (point0ScreenDivW.X > point1ScreenDivW.X)
            {
                Swap(ref point0ScreenDivW, ref point1ScreenDivW);
            }

            int gentleLeftX = (int)MathF.Round(point0ScreenDivW.X);
            int gentleLeftY = (int)MathF.Round(point0ScreenDivW.Y);

            int gentleRightX = (int)MathF.Round(point1ScreenDivW.X);
            int gentleRightY = (int)MathF.Round(point1ScreenDivW.Y);

            int width = gentleRightX - gentleLeftX;
            int height = gentleRightY - gentleLeftY;

            int derror2 = Math.Abs(height) * 2;
            int error2 = 0;

            int y = gentleLeftY;
            for (int x = gentleLeftX; x <= gentleRightX; x++)
            {
                if (isGentle)
                {
                    if (testDepth(x, y, point0ScreenDivW.Z))
                    {
                        setDepth(x, y, point0ScreenDivW.Z);
                        setPixel(x, y, pixelData);
                    }
                }
                else
                {
                    if (testDepth(y, x, point0ScreenDivW.Z))
                    {
                        setDepth(y, x, point0ScreenDivW.Z);
                        setPixel(y, x, pixelData);
                    }
                }
                error2 += derror2;

                if (error2 > width)
                {
                    y += height > 0 ? 1 : -1;
                    error2 -= width * 2;
                }
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