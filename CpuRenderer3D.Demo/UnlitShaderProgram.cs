using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class UnlitShaderProgram : IShaderProgram<UnlitFragmentData>
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

        public FragmentInput<UnlitFragmentData> ComputeVertex(VertexInput input, RenderingContext shaderContext)
        {
            return new FragmentInput<UnlitFragmentData>(
                Position: Vector4.Transform(input.Position, shaderContext.ModelProjection).XYZDivW(),
                Data: new UnlitFragmentData(input.Color, input.UV0));
        }

        public Vector4 ComputeColor(FragmentInput<UnlitFragmentData> input, RenderingContext shaderContext)
        {
            return input.Data.Color * AlbedoTexture.Sample(input.Data.UV0.X, input.Data.UV0.Y);
        }

        public FragmentInput<UnlitFragmentData> Add(FragmentInput<UnlitFragmentData> a, FragmentInput<UnlitFragmentData> b) => new FragmentInput<UnlitFragmentData>(a.Position + b.Position, a.Data + b.Data);
        public FragmentInput<UnlitFragmentData> Subtract(FragmentInput<UnlitFragmentData> a, FragmentInput<UnlitFragmentData> b) => new FragmentInput<UnlitFragmentData>(a.Position - b.Position, a.Data - b.Data);
        public FragmentInput<UnlitFragmentData> Multiply(FragmentInput<UnlitFragmentData> a, float f) => new FragmentInput<UnlitFragmentData>(a.Position * f, a.Data * f);
        public FragmentInput<UnlitFragmentData> Divide(FragmentInput<UnlitFragmentData> a, float f) => new FragmentInput<UnlitFragmentData>(a.Position / f, a.Data / f);
    }
}
