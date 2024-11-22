using System.Numerics;

namespace CpuRenderer3D.Shaders
{
    public class LitShaderProgram : IShaderProgram<LitFragmentData>
    {
        public Buffer<Vector4> DiffuseTexture;
        public Vector4 AmbientColor;

        public LitShaderProgram(Vector4 diffuseColor, Vector4 ambientColor)
        {
            DiffuseTexture = new Buffer<Vector4>(1, 1, diffuseColor);
            AmbientColor = ambientColor;
        }

        public LitShaderProgram(Buffer<Vector4> diffuseTexture, Vector4 ambientColor)
        {
            DiffuseTexture = diffuseTexture;
            AmbientColor = ambientColor;
        }

        public FragmentInput<LitFragmentData> ComputeVertex(VertexInput input, RenderingContext shaderContext)
        {
            Vector3 position = Vector4.Transform(input.Position, shaderContext.ModelProjection).XYZDivW();
            Vector3 normal = Vector4.Transform(input.Position + input.Normal, shaderContext.ModelProjection).XYZDivW() - position;

            return new FragmentInput<LitFragmentData>(
                Position: position,
                Data: new LitFragmentData(normal, input.Color, input.UV0));
        }

        public Vector4 ComputeColor(FragmentInput<LitFragmentData> input, RenderingContext shaderContext)
        {
            Vector3 normal = Vector3.Normalize(input.Data.Normal);

            Vector4 selfColor = DiffuseTexture.Sample(input.Data.UV0.X, input.Data.UV0.Y);

            Vector4 ambient = selfColor * AmbientColor;
            Vector4 diffuse = selfColor * Vector3.Dot(normal, -Vector3.UnitZ);
            return AmbientColor + diffuse;
        }

        public FragmentInput<LitFragmentData> Add(FragmentInput<LitFragmentData> a, FragmentInput<LitFragmentData> b) => new FragmentInput<LitFragmentData>(a.Position + b.Position, a.Data + b.Data);
        public FragmentInput<LitFragmentData> Subtract(FragmentInput<LitFragmentData> a, FragmentInput<LitFragmentData> b) => new FragmentInput<LitFragmentData>(a.Position - b.Position, a.Data - b.Data);
        public FragmentInput<LitFragmentData> Multiply(FragmentInput<LitFragmentData> a, float f) => new FragmentInput<LitFragmentData>(a.Position * f, a.Data * f);
        public FragmentInput<LitFragmentData> Divide(FragmentInput<LitFragmentData> a, float f) => new FragmentInput<LitFragmentData>(a.Position / f, a.Data / f);
    }
}
