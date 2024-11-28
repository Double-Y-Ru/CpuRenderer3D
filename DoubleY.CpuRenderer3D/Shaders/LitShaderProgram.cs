using System;
using System.Numerics;

namespace DoubleY.CpuRenderer3D.Shaders
{
    public class LitShaderProgram : IShaderProgram<LitFragmentData>, IInterpolator<FragmentInput<LitFragmentData>>
    {
        public Buffer<Vector4> DiffuseTexture;
        public Buffer<float> SpecularTexture;
        public Vector4 AmbientColor;
        public Vector3 LightDirection = Vector3.UnitZ; // light direction in view coordinates
        public Vector4 LightColor;

        public LitShaderProgram(Buffer<Vector4> diffuseTexture, Buffer<float> specularTexture, Vector4 ambientColor, Vector4 lightColor)
        {
            DiffuseTexture = diffuseTexture;
            SpecularTexture = specularTexture;
            AmbientColor = ambientColor;
            LightColor = lightColor;
        }

        public FragmentInput<LitFragmentData> ComputeVertex(VertexInput input, RenderingContext shaderContext)
        {
            Vector4 positionClip = Vector4.Transform(input.Position, shaderContext.ModelClip);
            Vector3 normalClip = Vector3.TransformNormal(input.Normal, shaderContext.ModelClip);

            return new FragmentInput<LitFragmentData>(
                Position: positionClip,
                Data: new LitFragmentData(normalClip, input.Color, input.UV0));
        }

        public Vector4 ComputeColor(FragmentInput<LitFragmentData> input, RenderingContext shaderContext)
        {
            Vector3 normal = Vector3.Normalize(input.Data.Normal);

            float diffuseIntensity = MathF.Max(0f, Vector3.Dot(normal, -LightDirection));
            Vector4 diffuseLightColor = LightColor * diffuseIntensity;

            Vector4 diffuseColor = DiffuseTexture.Sample(input.Data.UV0);
            float specValue = SpecularTexture.Sample(input.Data.UV0);

            Vector3 reflectedLightDirection = Vector3.Reflect(-LightDirection, normal); // reflected light direction, specular mapping is described here: https://github.com/ssloy/tinyrenderer/wiki/Lesson-6-Shaders-for-the-software-renderer

            //Vector3 viewDir = Vector3.Normalize(input.Position - Vector3.Zero);
            Vector3 viewDir = Vector3.UnitZ;

            float specIntensity = MathF.Pow(MathF.Max(Vector3.Dot(viewDir, reflectedLightDirection), 0.0f), 32);
            Vector4 specLightColor = specIntensity * specValue * LightColor;

            return Vector4.Clamp((AmbientColor + diffuseLightColor + specLightColor) * diffuseColor, Vector4.UnitW, Vector4.One);
        }

        public FragmentInput<LitFragmentData> Add(FragmentInput<LitFragmentData> a, FragmentInput<LitFragmentData> b) => new FragmentInput<LitFragmentData>(a.Position + b.Position, a.Data + b.Data);
        public FragmentInput<LitFragmentData> Subtract(FragmentInput<LitFragmentData> a, FragmentInput<LitFragmentData> b) => new FragmentInput<LitFragmentData>(a.Position - b.Position, a.Data - b.Data);
        public FragmentInput<LitFragmentData> Multiply(FragmentInput<LitFragmentData> a, float f) => new FragmentInput<LitFragmentData>(a.Position * f, a.Data * f);
        public FragmentInput<LitFragmentData> Divide(FragmentInput<LitFragmentData> a, float f) => new FragmentInput<LitFragmentData>(a.Position / f, a.Data / f);

        public FragmentInput<LitFragmentData> InterpolateBary(FragmentInput<LitFragmentData> point0, FragmentInput<LitFragmentData> point1, FragmentInput<LitFragmentData> point2, Vector3 bary)
        {
            return Add(Add(Multiply(point0, bary.X), Multiply(point1, bary.Y)), Multiply(point2, bary.Z));
        }
    }
}
