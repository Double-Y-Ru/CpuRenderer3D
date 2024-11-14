using System.Numerics;

namespace CpuRenderer3D
{
    public class UnlitShaderProgram : IShaderProgram
    {
        public FragmentInput ComputeVertex(VertexInput input, RenderingContext shaderContext)
        {
            return new FragmentInput(
                Position: Vector4.Transform(input.Position, shaderContext.ModelProjection).XYZDivW(),
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
