using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class ProjShaderProgram : IShaderProgram<Vector4>
    {
        public FragmentInput<Vector4> ComputeVertex(VertexInput input, RenderingContext shaderContext)
        {
            return new FragmentInput<Vector4>(
                new Vector4(input.Position, 1f),
                input.Color);
        }

        public Vector4 ComputeColor(FragmentInput<Vector4> input, RenderingContext shaderContext)
        {
            return input.Data;
        }

        public FragmentInput<Vector4> Add(FragmentInput<Vector4> a, FragmentInput<Vector4> b) => new FragmentInput<Vector4>(a.Position + b.Position, a.Data + b.Data);
        public FragmentInput<Vector4> Subtract(FragmentInput<Vector4> a, FragmentInput<Vector4> b) => new FragmentInput<Vector4>(a.Position - b.Position, a.Data - b.Data);
        public FragmentInput<Vector4> Multiply(FragmentInput<Vector4> a, float f) => new FragmentInput<Vector4>(a.Position * f, a.Data * f);
        public FragmentInput<Vector4> Divide(FragmentInput<Vector4> a, float f) => new FragmentInput<Vector4>(a.Position / f, a.Data / f);
    }
}
