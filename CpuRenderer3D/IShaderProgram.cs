using System.Numerics;

namespace CpuRenderer3D
{
    public interface IShaderProgram<TFragmentData> where TFragmentData : struct
    {
        FragmentInput<TFragmentData> ComputeVertex(VertexInput input, RenderingContext shaderContext);
        Vector4 ComputeColor(FragmentInput<TFragmentData> input, RenderingContext shaderContext);

        FragmentInput<TFragmentData> Add(FragmentInput<TFragmentData> a, FragmentInput<TFragmentData> b);
        FragmentInput<TFragmentData> Subtract(FragmentInput<TFragmentData> a, FragmentInput<TFragmentData> b);
        FragmentInput<TFragmentData> Multiply(FragmentInput<TFragmentData> a, float f);
        FragmentInput<TFragmentData> Divide(FragmentInput<TFragmentData> a, float f);
    }
}