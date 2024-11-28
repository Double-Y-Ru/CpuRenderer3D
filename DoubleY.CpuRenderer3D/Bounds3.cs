using System;
using System.Numerics;

namespace DoubleY.CpuRenderer3D
{
    public record struct Bounds3(Vector3 Min, Vector3 Max)
    {
        public static Bounds3 FromPoints(params Vector3[] points)
        {
            if (points.Length == 0) return new Bounds3();

            Bounds3 bounds = new Bounds3(points[0], points[0]);

            for (int i = 1; i < points.Length; i++)
            {
                bounds.Min = new Vector3(MathF.Min(bounds.Min.X, points[i].X),
                                         MathF.Min(bounds.Min.Y, points[i].Y),
                                         MathF.Min(bounds.Min.Z, points[i].Z));

                bounds.Max = new Vector3(MathF.Max(bounds.Max.X, points[i].X),
                                         MathF.Max(bounds.Max.Y, points[i].Y),
                                         MathF.Max(bounds.Max.Z, points[i].Z));
            }
            return bounds;
        }

        public readonly bool Contains(Vector3 point)
        {
            return Min.X < point.X && point.X < Max.X
                && Min.Y < point.Y && point.Y < Max.Y
                && Min.Z < point.Z && point.Z < Max.Z;
        }

        public void Expand(Vector3 point)
        {
            Min = new Vector3(MathF.Min(Min.X, point.X),
                              MathF.Min(Min.Y, point.Y),
                              MathF.Min(Min.Z, point.Z));
            Max = new Vector3(MathF.Max(Max.X, point.X),
                              MathF.Max(Max.Y, point.Y),
                              MathF.Max(Max.Z, point.Z));
        }

        public void Expand(float x, float y, float z)
        {
            Min = new Vector3(MathF.Min(Min.X, x),
                              MathF.Min(Min.Y, y),
                              MathF.Min(Min.Z, z));
            Max = new Vector3(MathF.Max(Max.X, x),
                              MathF.Max(Max.Y, y),
                              MathF.Max(Max.Z, z));
        }

        public static Bounds3 Intersect(Bounds3 boundsA, Bounds3 boundsB)
        {
            return new Bounds3(
                Min: new Vector3(MathF.Max(boundsA.Min.X, boundsB.Min.X),
                                 MathF.Max(boundsA.Min.Y, boundsB.Min.Y),
                                 MathF.Max(boundsA.Min.Z, boundsB.Min.Z)),
                Max: new Vector3(MathF.Min(boundsA.Max.X, boundsB.Max.X),
                                 MathF.Min(boundsA.Max.Y, boundsB.Max.Y),
                                 MathF.Min(boundsA.Max.Z, boundsB.Max.Z)));
        }
    }
}