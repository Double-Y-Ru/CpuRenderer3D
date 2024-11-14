using System.Numerics;

namespace CpuRenderer3D
{
    public interface IShaderProgram
    {
        Vector4 ComputeColor(FragmentInput input, RenderingContext shaderContext);
        FragmentInput ComputeVertex(VertexInput input, RenderingContext shaderContext);
    }
}