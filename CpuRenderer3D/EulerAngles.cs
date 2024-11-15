using System.Numerics;

namespace CpuRenderer3D
{
    public static class EulerAngles
    {
        public static Quaternion EulerToQuaternion(Vector3 v)
        {
            return EulerToQuaternion(v.Y, v.X, v.Z);
        }
        public static Quaternion EulerToQuaternion(float yaw, float pitch, float roll)
        {
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)MathF.Sin(rollOver2);
            float cosRollOver2 = (float)MathF.Cos(rollOver2);

            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)MathF.Sin(pitchOver2);
            float cosPitchOver2 = (float)MathF.Cos(pitchOver2);

            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)MathF.Sin(yawOver2);
            float cosYawOver2 = (float)MathF.Cos(yawOver2);

            return new Quaternion(
                w: cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2,
                x: cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2,
                y: sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2,
                z: cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2);
        }

        public static Vector3 QuaternionToEuler(Quaternion q1)
        {
            float sqw = q1.W * q1.W;
            float sqx = q1.X * q1.X;
            float sqy = q1.Y * q1.Y;
            float sqz = q1.Z * q1.Z;
            float unit = sqx + sqy + sqz + sqw; // if normalised is one, otherwise is correction factor
            float test = q1.X * q1.W - q1.Y * q1.Z;
            Vector3 v;
            if (test > 0.4995f * unit)
            { // singularity at north pole
                v.Y = 2f * MathF.Atan2(q1.Y, q1.X);
                v.X = 0.5f * MathF.PI;
                v.Z = 0;
                return NormalizeAngles(v);
            }
            if (test < -0.4995f * unit)
            { // singularity at south pole
                v.Y = -2f * MathF.Atan2(q1.Y, q1.X);
                v.X = -0.5f * MathF.PI;
                v.Z = 0;
                return NormalizeAngles(v);
            }
            Quaternion q = new Quaternion(q1.W, q1.Z, q1.X, q1.Y);
            v.Y = (float)Math.Atan2(2f * q.X * q.W + 2f * q.Y * q.Z, 1 - 2f * (q.Z * q.Z + q.W * q.W));     // Yaw
            v.X = (float)Math.Asin(2f * (q.X * q.Z - q.W * q.Y));                                           // Pitch
            v.Z = (float)Math.Atan2(2f * q.X * q.Y + 2f * q.Z * q.W, 1 - 2f * (q.Y * q.Y + q.Z * q.Z));     // Roll
            return NormalizeAngles(v);
        }

        static Vector3 NormalizeAngles(Vector3 angles)
        {
            angles.X = NormalizeAngle(angles.X);
            angles.Y = NormalizeAngle(angles.Y);
            angles.Z = NormalizeAngle(angles.Z);
            return angles;
        }

        static float NormalizeAngle(float angle)
        {
            while (angle > 2f * MathF.PI)
                angle -= 2f * MathF.PI;
            while (angle < 0)
                angle += 2f * MathF.PI;
            return angle;
        }
    }
}
