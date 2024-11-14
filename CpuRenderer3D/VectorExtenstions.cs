using System.Numerics;

namespace CpuRenderer3D
{
    public static class VectorExtenstions
    {
        public static Vector2 XY(this Vector4 vec4)
        {
            return new Vector2(vec4.X, vec4.Y);
        }

        public static Vector3 XYZ(this Vector4 vec4)
        {
            return new Vector3(vec4.X, vec4.Y, vec4.Z);
        }

        public static Vector3 XYZDivW(this Vector4 vec4)
        {
            return new Vector3(vec4.X / vec4.W, vec4.Y / vec4.W, vec4.Z / vec4.W);
        }
    }
}
