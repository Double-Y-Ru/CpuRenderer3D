namespace CpuRenderer3D
{
    public interface IInterpolator<TFragmentData> where TFragmentData : struct
    {
        FragmentInput<TFragmentData> Add(FragmentInput<TFragmentData> a, FragmentInput<TFragmentData> b);
        FragmentInput<TFragmentData> Subtract(FragmentInput<TFragmentData> a, FragmentInput<TFragmentData> b);
        FragmentInput<TFragmentData> Multiply(FragmentInput<TFragmentData> a, float f);
        FragmentInput<TFragmentData> Divide(FragmentInput<TFragmentData> a, float f);
    }
}