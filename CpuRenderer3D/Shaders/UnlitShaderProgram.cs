using System.Numerics;

namespace CpuRenderer3D.Shaders
{
    public class UnlitShaderProgram : IShaderProgram<UnlitFragmentData>, IInterpolator<UnlitFragmentData>
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
                Position: Vector4.Transform(input.Position, shaderContext.ModelClip),
                Data: new UnlitFragmentData(input.Color, input.UV0));
        }

        public Vector4 ComputeColor(FragmentInput<UnlitFragmentData> input, RenderingContext shaderContext)
        {
            return input.Data.Color * AlbedoTexture.Sample(input.Data.UV0);
        }

        public FragmentInput<UnlitFragmentData> Add(FragmentInput<UnlitFragmentData> a, FragmentInput<UnlitFragmentData> b) => new FragmentInput<UnlitFragmentData>(a.Position + b.Position, a.Data + b.Data);
        public FragmentInput<UnlitFragmentData> Subtract(FragmentInput<UnlitFragmentData> a, FragmentInput<UnlitFragmentData> b) => new FragmentInput<UnlitFragmentData>(a.Position - b.Position, a.Data - b.Data);
        public FragmentInput<UnlitFragmentData> Multiply(FragmentInput<UnlitFragmentData> a, float f) => new FragmentInput<UnlitFragmentData>(a.Position * f, a.Data * f);
        public FragmentInput<UnlitFragmentData> Divide(FragmentInput<UnlitFragmentData> a, float f) => new FragmentInput<UnlitFragmentData>(a.Position / f, a.Data / f);

        public FragmentInput<UnlitFragmentData> InterpolateBary(FragmentInput<UnlitFragmentData> point0, FragmentInput<UnlitFragmentData> point1, FragmentInput<UnlitFragmentData> point2, Vector3 bary)
        {
            return Add(Add(Multiply(point0, bary.X), Multiply(point1, bary.Y)), Multiply(point2, bary.Z));
        }
    }
}
