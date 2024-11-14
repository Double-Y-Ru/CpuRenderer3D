using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CpuRenderer3D.Demo
{
    public class ShaderGL : IDisposable
    {
        private readonly int _handle;
        private readonly int _vertexShader;
        private readonly int _fragmentShader;

        public ShaderGL(string vertexShaderSource, string fragmentShaderSource)
        {
            _vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(_vertexShader, vertexShaderSource);
            GL.CompileShader(_vertexShader);
            GL.GetShader(_vertexShader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(_vertexShader);
                Console.WriteLine(infoLog);
            }

            _fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_fragmentShader, fragmentShaderSource);
            GL.CompileShader(_fragmentShader);
            GL.GetShader(_fragmentShader, ShaderParameter.CompileStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetShaderInfoLog(_fragmentShader);
                Console.WriteLine(infoLog);
            }

            _handle = GL.CreateProgram();
            GL.AttachShader(_handle, _vertexShader);
            GL.AttachShader(_handle, _fragmentShader);
            GL.LinkProgram(_handle);
            GL.GetProgram(_handle, GetProgramParameterName.LinkStatus, out success);
            if (success == 0)
            {
                string infoLog = GL.GetProgramInfoLog(_handle);
                Console.WriteLine(infoLog);
            }
        }

        public void Dispose()
        {
            GL.DetachShader(_handle, _fragmentShader);
            GL.DetachShader(_handle, _fragmentShader);
            GL.DeleteProgram(_handle);
        }

        public void Use()
        {
            GL.UseProgram(_handle);
        }

        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(_handle, attribName);
        }

        public void SetMatrix4(string name, Matrix4 value)
        {
            int location = GL.GetUniformLocation(_handle, name);
            GL.UniformMatrix4(location, false, ref value);
        }
    }
}
