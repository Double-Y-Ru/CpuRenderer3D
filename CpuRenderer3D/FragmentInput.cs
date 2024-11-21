using System.Numerics;

namespace CpuRenderer3D
{
    public record struct FragmentInput<TFragmentData>(Vector3 Position, TFragmentData Data) where TFragmentData : struct;
}
