using System.Numerics;

namespace CpuRenderer3D
{
    public static class Util
    {
        public static Matrix4x4 CreateProjectionClip(int bufferWidth, int bufferHeight)
        {
            return new Matrix4x4(
                0.5f * bufferWidth, 0, 0, 0,
                0, 0.5f * bufferHeight, 0, 0,
                0, 0, 1, 0,
                0.5f * bufferWidth, 0.5f * bufferHeight, 0, 1);
        }
    }
}
