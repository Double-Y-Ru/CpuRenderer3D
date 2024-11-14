using System.Numerics;

namespace CpuRenderer3D
{
    internal struct Vector3iif
    {
        public int X;
        public int Y;
        public float Z;

        public Vector3iif(Vector3 v) : this((int)float.Round(v.X), (int)float.Round(v.Y), v.Z)
        {
        }

        public Vector3iif(int x, int y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3iif operator +(Vector3iif left, Vector3iif right)
        {
            return new Vector3iif(
                left.X + right.X,
                left.Y + right.Y,
                left.Z + right.Z
            );
        }

        public static Vector3iif operator -(Vector3iif left, Vector3iif right)
        {
            return new Vector3iif(
                left.X - right.X,
                left.Y - right.Y,
                left.Z - right.Z
            );
        }

        public static Vector3iif operator *(Vector3iif left, float right)
        {
            return new Vector3iif(
                (int)float.Round(left.X * right),
                (int)float.Round(left.Y * right),
                left.Z * right
            );
        }
    }
}