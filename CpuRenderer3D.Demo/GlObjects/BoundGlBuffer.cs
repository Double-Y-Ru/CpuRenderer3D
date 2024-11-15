using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL4;

namespace CpuRenderer3D.Demo.GlObjects
{
    public class BoundGlBuffer : IDisposable
    {
        private readonly BufferTarget _bufferTarget;

        public BoundGlBuffer(int handle, BufferTarget bufferTarget)
        {
            _bufferTarget = bufferTarget;
            GL.BindBuffer(_bufferTarget, handle);
        }

        public void SetData<T>(int size, [In][Out] T[] data, BufferUsageHint usage) where T : struct
        {
            GL.BufferData(_bufferTarget, size, data, usage);
        }

        public void Dispose()
        {
            GL.BindBuffer(_bufferTarget, 0);
        }
    }
}
