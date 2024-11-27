using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ColorInterpolator : IInterpolator<Vector4>
    {
        public Vector4 InterpolateBary(Vector4 color0, Vector4 color1, Vector4 color2, Vector3 bary) => color0 * bary.X + color1 * bary.Y + color2 * bary.Z;
    }
}