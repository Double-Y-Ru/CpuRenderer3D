using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class LitShaderProgram : IShaderProgram<UnlitFragmentData>
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

        public FragmentInput<UnlitFragmentData> ComputeVertex(VertexInput input, RenderingContext shaderContext)
        {
            Vector3 position = Vector4.Transform(input.Position, shaderContext.ModelProjection).XYZDivW();
            Vector3 normal = Vector4.Transform(input.Position + input.Normal, shaderContext.ModelProjection).XYZDivW() - position;

            return new FragmentInput<UnlitFragmentData>(
                Position: position,
                Data: new UnlitFragmentData(normal, input.Color, input.UV0));
        }

        public Vector4 ComputeColor(FragmentInput<UnlitFragmentData> input, RenderingContext shaderContext)
        {
            Vector3 normal = Vector3.Normalize(input.Data.Normal);

            Vector4 selfColor = input.Data.Color * DiffuseTexture.Sample(input.Data.UV0.X, input.Data.UV0.Y);

            Vector4 ambient = selfColor * AmbientColor;
            Vector4 diffuse = selfColor * Vector3.Dot(normal, -Vector3.UnitZ);
            return ambient + diffuse;
        }

        public FragmentInput<UnlitFragmentData> Add(FragmentInput<UnlitFragmentData> a, FragmentInput<UnlitFragmentData> b) => new FragmentInput<UnlitFragmentData>(a.Position + b.Position, a.Data + b.Data);
        public FragmentInput<UnlitFragmentData> Subtract(FragmentInput<UnlitFragmentData> a, FragmentInput<UnlitFragmentData> b) => new FragmentInput<UnlitFragmentData>(a.Position - b.Position, a.Data - b.Data);
        public FragmentInput<UnlitFragmentData> Multiply(FragmentInput<UnlitFragmentData> a, float f) => new FragmentInput<UnlitFragmentData>(a.Position * f, a.Data * f);
        public FragmentInput<UnlitFragmentData> Divide(FragmentInput<UnlitFragmentData> a, float f) => new FragmentInput<UnlitFragmentData>(a.Position / f, a.Data / f);
    }
}
