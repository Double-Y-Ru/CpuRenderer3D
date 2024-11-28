using OpenTK.Graphics.OpenGL4;

namespace DoubleY.CpuRenderer3D.Demo.GlObjects
{
    public class BoundGlVertexArray : IDisposable
    {
        public BoundGlVertexArray(int array)
        {
            GL.BindVertexArray(array);
        }

        public void Dispose()
        {
            GL.BindVertexArray(0);
        }
    }
}
