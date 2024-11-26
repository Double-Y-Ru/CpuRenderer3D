using System.Numerics;

namespace CpuRenderer3D
{
    public interface IInterpolator<TInterpolatedData> where TInterpolatedData : struct
    {
        TInterpolatedData Add(TInterpolatedData a, TInterpolatedData b);
        TInterpolatedData Subtract(TInterpolatedData a, TInterpolatedData b);
        TInterpolatedData Multiply(TInterpolatedData a, float f);
        TInterpolatedData Divide(TInterpolatedData a, float f);
        TInterpolatedData InterpolateBary(TInterpolatedData point0, TInterpolatedData point1, TInterpolatedData point2, Vector3 bary);
    }
}