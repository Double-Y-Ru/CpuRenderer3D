using System.Numerics;

namespace CpuRenderer3D.Shaders
{
    public record struct LitFragmentData(Vector3 Normal, Vector4 Color, Vector2 UV0)
    {
        public static LitFragmentData operator +(LitFragmentData a, LitFragmentData b)
        {
            return new LitFragmentData(
                a.Normal + b.Normal,
                a.Color + b.Color,
                a.UV0 + b.UV0);
        }

        public static LitFragmentData operator -(LitFragmentData a, LitFragmentData b)
        {
            return new LitFragmentData(
                a.Normal - b.Normal,
                a.Color - b.Color,
                a.UV0 - b.UV0);
        }

        public static LitFragmentData operator -(LitFragmentData a)
        {
            return new LitFragmentData(
                -a.Normal,
                -a.Color,
                -a.UV0);
        }

        public static LitFragmentData operator *(LitFragmentData a, float b)
        {
            return new LitFragmentData(
                a.Normal * b,
                a.Color * b,
                a.UV0 * b);
        }

        public static LitFragmentData operator *(float b, LitFragmentData a)
        {
            return a * b;
        }

        public static LitFragmentData operator /(LitFragmentData a, float b)
        {
            return new LitFragmentData(
                a.Normal / b,
                a.Color / b,
                a.UV0 / b);
        }
    }
}
