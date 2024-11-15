using OpenTK.Graphics.OpenGL4;
using System.Numerics;

namespace CpuRenderer3D.Demo.GlObjects
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

        public void SetImage(Bytemap bytemap)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bytemap.Width, bytemap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bytemap.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void UpdateImage(Bytemap bytemap)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bytemap.Width, bytemap.Height, PixelFormat.Rgba, PixelType.UnsignedByte, bytemap.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void UpdateImage(Buffer<Vector4> buffer)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, buffer.Width, buffer.Height, PixelFormat.Rgba, PixelType.Float, buffer.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }
    }
}
