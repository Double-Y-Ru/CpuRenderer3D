using System.Numerics;

namespace CpuRenderer3D
{
    public record struct FragmentInput(
        Vector3 Position,
        Vector3 Normal,
        Vector4 Color,
        Vector2 UV0,
        Vector2 UV1,
        Vector2 UV2,
        Vector2 UV3)
    {
        public void RoundXYPos()
        {
            Position = new Vector3(float.Round(Position.X), float.Round(Position.Y), Position.Z);
        }

        public static FragmentInput operator +(FragmentInput a, FragmentInput b)
        {
            return new FragmentInput(
                a.Position + b.Position,
                a.Normal + b.Normal,
                a.Color + b.Color,
                a.UV0 + b.UV0,
                a.UV1 + b.UV1,
                a.UV2 + b.UV2,
                a.UV3 + b.UV3);
        }

        public static FragmentInput operator -(FragmentInput a, FragmentInput b)
        {
            return new FragmentInput(
                a.Position - b.Position,
                a.Normal - b.Normal,
                a.Color - b.Color,
                a.UV0 - b.UV0,
                a.UV1 - b.UV1,
                a.UV2 - b.UV2,
                a.UV3 - b.UV3);
        }

        public static FragmentInput operator -(FragmentInput a)
        {
            return new FragmentInput(
                -a.Position,
                -a.Normal,
                -a.Color,
                -a.UV0,
                -a.UV1,
                -a.UV2,
                -a.UV3);
        }

        public static FragmentInput operator *(FragmentInput a, float b)
        {
            return new FragmentInput(
                a.Position * b,
                a.Normal * b,
                a.Color * b,
                a.UV0 * b,
                a.UV1 * b,
                a.UV2 * b,
                a.UV3 * b);
        }

        public static FragmentInput operator *(float b, FragmentInput a)
        {
            return a * b;
        }

        public static FragmentInput operator /(FragmentInput a, float b)
        {
            return new FragmentInput(
                a.Position / b,
                a.Normal / b,
                a.Color / b,
                a.UV0 / b,
                a.UV1 / b,
                a.UV2 / b,
                a.UV3 / b);
        }
    }
}
