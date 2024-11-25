using System.Numerics;

namespace CpuRenderer3D
{
    public interface IShaderProgram<TFragmentData> where TFragmentData : struct
    {
        FragmentInput<TFragmentData> ComputeVertex(VertexInput input, RenderingContext renderingContext);
        Vector4 ComputeColor(FragmentInput<TFragmentData> input, RenderingContext renderingContext);
    }
}