using System.Numerics;

namespace CpuRenderer3D
{
    public interface IShaderProgram
    {
        FragmentInput ComputeVertex(VertexInput input, RenderingContext shaderContext);
        Vector4 ComputeColor(FragmentInput input, RenderingContext shaderContext);
    }
}