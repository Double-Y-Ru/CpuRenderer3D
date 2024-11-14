using OpenTK.Graphics.OpenGL4;

namespace CpuRenderer3D
{
    public class Texture
    {
        private int Handle;

        public void Init()
        {
            Handle = GL.GenTexture();

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToBorder);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToBorder);

            /*float[] borderColor = { 0.0f, 0.0f, 0.0f, 0.0f };
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, borderColor);*/
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        public void Bind(byte[] bytemap, int width, int height)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                width, height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bytemap);
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Replace(byte[] bytemap, int width, int height)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, width, height, PixelFormat.Rgba,
                PixelType.UnsignedByte, bytemap);
        }

        public void Bind(Bytemap bytemap)
        {
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba,
                bytemap.Width, bytemap.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, bytemap.GetData());
            GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
        }

        public void Replace(Bytemap bytemap)
        {
            GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, bytemap.Width, bytemap.Height, PixelFormat.Rgba,
                PixelType.UnsignedByte, bytemap.GetData());
        }
    }
}
