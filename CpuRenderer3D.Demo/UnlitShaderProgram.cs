using System.Numerics;

namespace CpuRenderer3D
{
    public class UnlitShaderProgram : IShaderProgram
    {
        public Buffer<Vector4> AlbedoTexture;

        public UnlitShaderProgram(Vector4 albedoColor)
        {
            AlbedoTexture = new Buffer<Vector4>(1, 1, albedoColor);
        }

        public UnlitShaderProgram(Buffer<Vector4> albedoTexture)
        {
            AlbedoTexture = albedoTexture;
        }

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
            Vector4 sample = AlbedoTexture.Sample(input.UV0.X, input.UV0.Y);
            return input.Color * sample;
        }
    }
}
