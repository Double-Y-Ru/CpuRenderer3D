using OpenTK.Graphics.OpenGL4;
using System.Numerics;

namespace DoubleY.CpuRenderer3D.Demo.GlObjects
{
    public class BoundGlTexture : IDisposable
    {
        public readonly int Handle;

        public BoundGlTexture(int handle)
        {
            Handle = handle;
            GL.BindTexture(TextureTarget.Texture2D, Handle);
        }

        public void Dispose()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void SetupParameters()
        {
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
        }

        public void SetImage(Buffer<Vector4> buffer)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, buffer.Width, buffer.Height, 0, PixelFormat.Rgba, PixelType.Float, buffer.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void SetImage(Buffer<int> buffer)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, buffer.Width, buffer.Height, 0, PixelFormat.RgbaInteger, PixelType.UnsignedInt, buffer.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void UpdateImage(Buffer<Vector4> buffer)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, buffer.Width, buffer.Height, PixelFormat.Rgba, PixelType.Float, buffer.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void UpdateImage(Buffer<float> buffer)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, buffer.Width, buffer.Height, PixelFormat.Luminance, PixelType.Float, buffer.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void UpdateImage(Buffer<int> buffer)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, buffer.Width, buffer.Height, PixelFormat.RgbaInteger, PixelType.UnsignedInt, buffer.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}
