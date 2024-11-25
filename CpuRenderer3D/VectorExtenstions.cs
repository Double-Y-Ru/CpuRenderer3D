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

        public static Vector2 XYDivW(this Vector4 vec4)
        {
            return new Vector2(vec4.X / vec4.W, vec4.Y / vec4.W);
        }

        public static float Cross(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - b.X * a.Y;
        }
    }
}
