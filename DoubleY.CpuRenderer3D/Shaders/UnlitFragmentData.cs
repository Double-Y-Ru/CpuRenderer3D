using System.Numerics;

namespace DoubleY.CpuRenderer3D.Shaders
{
    public record struct UnlitFragmentData(Vector4 Color, Vector2 UV0)
    {
        public static UnlitFragmentData operator +(UnlitFragmentData a, UnlitFragmentData b)
        {
            return new UnlitFragmentData(
                a.Color + b.Color,
                a.UV0 + b.UV0);
        }

        public static UnlitFragmentData operator -(UnlitFragmentData a, UnlitFragmentData b)
        {
            return new UnlitFragmentData(
                a.Color - b.Color,
                a.UV0 - b.UV0);
        }

        public static UnlitFragmentData operator -(UnlitFragmentData a)
        {
            return new UnlitFragmentData(
                -a.Color,
                -a.UV0);
        }

        public static UnlitFragmentData operator *(UnlitFragmentData a, float b)
        {
            return new UnlitFragmentData(
                a.Color * b,
                a.UV0 * b);
        }

        public static UnlitFragmentData operator *(float b, UnlitFragmentData a)
        {
            return a * b;
        }

        public static UnlitFragmentData operator /(UnlitFragmentData a, float b)
        {
            return new UnlitFragmentData(
                a.Color / b,
                a.UV0 / b);
        }

    }
}
