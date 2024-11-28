using OpenTK.Graphics.OpenGL4;

namespace DoubleY.CpuRenderer3D.Demo.GlObjects
{
    public class GlVertexArray : IDisposable
    {
        public readonly int Array;

        public GlVertexArray()
        {
            Array = GL.GenVertexArray();
        }

        public BoundGlVertexArray Bind()
        {
            return new BoundGlVertexArray(Array);
        }

        public void Dispose()
        {
            GL.DeleteVertexArray(Array);
        }
    }
}
