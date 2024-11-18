using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class CpuRendererAdapter : ICpuRenderer
    {
        private readonly CpuRenderer _cpuRenderer;

        public CpuRendererAdapter(CpuRenderer cpuRenderer)
        {
            _cpuRenderer = cpuRenderer;
        }

        public void Render(IReadOnlyList<Entity> entities, Camera camera, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer) =>
            _cpuRenderer.Render(entities, camera, colorBuffer, depthBuffer);
    }
}