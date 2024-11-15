using OpenTK.Graphics.OpenGL4;

namespace CpuRenderer3D.Demo.GlObjects
{
    public class GlBuffer : IDisposable
    {
        public readonly int Handle;

        public GlBuffer()
        {
            Handle = GL.GenBuffer();
        }

        public BoundGlBuffer Bind(BufferTarget bufferTarget)
        {
            return new BoundGlBuffer(Handle, bufferTarget);
        }

        public void Dispose()
        {
            GL.DeleteBuffer(Handle);
        }
    }
}
