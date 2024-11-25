using System.Numerics;

namespace CpuRenderer3D
{
    public interface IInterpolator<TFragmentData> where TFragmentData : struct
    {
        FragmentInput<TFragmentData> Add(FragmentInput<TFragmentData> a, FragmentInput<TFragmentData> b);
        FragmentInput<TFragmentData> Subtract(FragmentInput<TFragmentData> a, FragmentInput<TFragmentData> b);
        FragmentInput<TFragmentData> Multiply(FragmentInput<TFragmentData> a, float f);
        FragmentInput<TFragmentData> Divide(FragmentInput<TFragmentData> a, float f);
        FragmentInput<TFragmentData> InterpolateBary(FragmentInput<TFragmentData> point0, FragmentInput<TFragmentData> point1, FragmentInput<TFragmentData> point2, Vector3 bary);
    }
}