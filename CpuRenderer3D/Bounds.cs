using System;
using System.Numerics;

namespace CpuRenderer3D
{
    public record struct Bounds(Vector2 Min, Vector2 Max)
    {
        public static Bounds FromPoints(params Vector2[] points)
        {
            if (points.Length == 0) return new Bounds();

            Bounds bounds = new Bounds(points[0], points[0]);

            for (int i = 1; i < points.Length; i++)
            {
                bounds.Min = new Vector2(MathF.Min(bounds.Min.X, points[i].X),
                                         MathF.Min(bounds.Min.Y, points[i].Y));

                bounds.Max = new Vector2(MathF.Max(bounds.Max.X, points[i].X),
                                         MathF.Max(bounds.Max.Y, points[i].Y));
            }
            return bounds;
        }

        public readonly bool IsInside(Vector2 point)
        {
            return Min.X < point.X && point.X < Max.X
                && Min.Y < point.Y && point.Y < Max.Y;
        }

        public void Expand(Vector2 point)
        {
            Min = new Vector2(MathF.Min(Min.X, point.X),
                              MathF.Min(Min.Y, point.Y));
            Max = new Vector2(MathF.Max(Max.X, point.X),
                              MathF.Max(Max.Y, point.Y));
        }

        public void Expand(float x, float y)
        {
            Min = new Vector2(MathF.Min(Min.X, x),
                              MathF.Min(Min.Y, y));
            Max = new Vector2(MathF.Max(Max.X, x),
                              MathF.Max(Max.Y, y));
        }

        public static Bounds Intersect(Bounds boundsA, Bounds boundsB)
        {
            return new Bounds(
                Min: new Vector2(MathF.Max(boundsA.Min.X, boundsB.Min.X),
                                 MathF.Max(boundsA.Min.Y, boundsB.Min.Y)),
                Max: new Vector2(MathF.Min(boundsA.Max.X, boundsB.Max.X),
                                 MathF.Min(boundsA.Max.Y, boundsB.Max.Y)));
        }
    }
}