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

        public Matrix4x4 GetInvertedMatrix()
        {
            Matrix4x4.Invert(GetMatrix(), out Matrix4x4 inverted);
            return inverted;
        }

        public Vector3 UnitX() => Vector3.Transform(Vector3.UnitX, GetMatrix());
        public Vector3 UnitY() => Vector3.Transform(Vector3.UnitY, GetMatrix());
        public Vector3 UnitZ() => Vector3.Transform(Vector3.UnitZ, GetMatrix());

        public Vector3 FromLocal(Vector3 vector) => Vector3.Transform(vector, GetMatrix());
        public Vector4 FromLocal(Vector4 vector) => Vector4.Transform(vector, GetMatrix());

        public Vector3 ToLocal(Vector3 vector) => Vector3.Transform(vector, GetInvertedMatrix());
        public Vector4 ToLocal(Vector4 vector) => Vector4.Transform(vector, GetInvertedMatrix());

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