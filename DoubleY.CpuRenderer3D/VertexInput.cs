using System.Numerics;

namespace DoubleY.CpuRenderer3D
{
    public record struct VertexInput(
        Vector3 Position,
        Vector3 Normal,
        Vector4 Color,
        Vector2 UV0,
        Vector2 UV1,
        Vector2 UV2,
        Vector2 UV3);
}