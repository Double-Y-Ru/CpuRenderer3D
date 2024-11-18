using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public interface ICpuRenderer
    {
        void Render(IReadOnlyList<Entity> entities, Camera camera, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer);
    }
}