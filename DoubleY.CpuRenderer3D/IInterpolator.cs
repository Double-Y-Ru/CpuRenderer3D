using System.Numerics;

namespace DoubleY.CpuRenderer3D
{
    public interface IInterpolator<TInterpolatedData> where TInterpolatedData : struct
    {
        TInterpolatedData InterpolateBary(TInterpolatedData point0, TInterpolatedData point1, TInterpolatedData point2, Vector3 bary);
    }
}