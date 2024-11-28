using System;
using System.Numerics;

namespace DoubleY.CpuRenderer3D
{
    public static class BufferUtil
    {
        public static void SubtractColors(Buffer<Vector4> bufferA, Buffer<Vector4> bufferB, Buffer<Vector4> resultBuffer)
        {
            if (bufferA.Width != bufferB.Width || bufferA.Height != bufferB.Height || bufferA.Width != resultBuffer.Width || bufferA.Height != resultBuffer.Height)
                throw new ArgumentException();

            for (int y = 0; y < bufferA.Height; ++y)
                for (int x = 0; x < bufferA.Width; ++x)
                {
                    Vector4 colorA = bufferA.Get(x, y);
                    Vector4 colorB = bufferB.Get(x, y);
                    resultBuffer.Set(x, y, SubtractColors(colorA, colorB));
                }
        }

        private static Vector4 SubtractColors(Vector4 colorA, Vector4 colorB)
        {
            return new Vector4(MathF.Abs(colorA.X - colorB.X), MathF.Abs(colorA.Y - colorB.Y), MathF.Abs(colorA.Z - colorB.Z), colorA.W);
        }
    }
}
