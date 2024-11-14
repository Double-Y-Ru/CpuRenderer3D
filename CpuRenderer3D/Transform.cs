using System.Numerics;

namespace CpuRenderer3D
{
    public struct Transform : IEquatable<Transform>
    {
        public Vector3 Origin;
        public Quaternion Rotation;

        public Transform()
        {
            Origin = Vector3.Zero;
            Rotation = Quaternion.Identity;
        }

        public Transform(Vector3 origin, Quaternion rotation)
        {
            Origin = origin;
            Rotation = rotation;
        }

        public Matrix4x4 GetMatrix()
        {
            var a = Matrix4x4.CreateTranslation(Origin);
            var b = Matrix4x4.CreateFromQuaternion(Rotation);
            return b * a;
        }

        public static bool operator ==(Transform a, Transform b)
        {
            return a.Origin == b.Origin && a.Rotation == b.Rotation;
        }

        public static bool operator !=(Transform a, Transform b)
        {
            return !(a == b);
        }

        public override bool Equals(object? obj)
        {
            return (obj is Transform other) && Equals(other);
        }

        public bool Equals(Transform other)
        {
            return Origin.Equals(other.Origin) && Rotation.Equals(other.Rotation);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Origin, Rotation);
        }
    }
}