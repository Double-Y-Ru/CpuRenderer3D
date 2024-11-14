using OpenTK.Graphics.OpenGL4;
using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class Texture : IDisposable
    {
        private readonly int _handle;

        public Texture(byte[] bytemap, int width, int height) : this()
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bytemap);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public Texture(Vector4[] colors, int width, int height) : this()
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                width, height, 0, PixelFormat.Rgba, PixelType.Float, colors);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public Texture(Bytemap bytemap)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bytemap.Width, bytemap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bytemap.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        private Texture()
        {
            _handle = GL.GenTexture();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Dispose()
        {
            GL.DeleteTexture(_handle);
        }


        public void Replace(byte[] bytemap, int width, int height)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Rgba,
                PixelType.UnsignedByte, bytemap);
        }

        public void Replace(float[] floatmap, int width, int height)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Rgba,
                PixelType.Float, floatmap);
        }

        public void Replace(Vector4[] colors, int width, int height)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Rgba,
                PixelType.Float, colors);
        }

        public void Replace(Bytemap bytemap)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bytemap.Width, bytemap.Height, PixelFormat.Rgba,
                PixelType.UnsignedByte, bytemap.GetData());
        }
    }
}
