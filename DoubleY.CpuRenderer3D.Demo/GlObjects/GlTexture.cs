using OpenTK.Graphics.OpenGL4;

namespace DoubleY.CpuRenderer3D.Demo.GlObjects
{
    public class GlTexture : IDisposable
    {
        public readonly int Handle;

        public GlTexture()
        {
            Handle = GL.GenTexture();
        }

        public BoundGlTexture Bind()
        {
            return new BoundGlTexture(Handle);
        }

        public void Dispose()
        {
            GL.DeleteTexture(Handle);
        }
    }
}
