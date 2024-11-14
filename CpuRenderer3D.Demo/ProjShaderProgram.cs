using System.Numerics;

namespace CpuRenderer3D
{
    public class ProjShaderProgram : IShaderProgram
    {
        public FragmentInput ComputeVertex(VertexInput input, RenderingContext shaderContext)
        {
            return new FragmentInput(
                Position: input.Position,
                input.Normal,
                input.Color,
                input.UV0,
                input.UV1,
                input.UV2,
                input.UV3);
        }

        public Vector4 ComputeColor(FragmentInput input, RenderingContext shaderContext)
        {
            return input.Color;
        }
    }
}
