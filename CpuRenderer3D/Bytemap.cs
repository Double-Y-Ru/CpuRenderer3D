using System.Drawing;

namespace CpuRenderer3D
{
    public class Bytemap
    {
        public readonly int Width;
        public readonly int Height;

        private readonly byte[] data;
        private readonly float[] zbuffer;

        public Bytemap(int width, int height)
        {
            Width = width;
            Height = height;
            data = new byte[width*height*4];
            
            zbuffer = new float[width*height];
            for (int i = 0; i < zbuffer.Length; i++)
                zbuffer[i] = 1f;
        }

        public bool TryGetPixel(int x, int y, out Color color)
        {
            if (-1 < x && x < Width && -1 < y && y < Height)
            {
                int index = (y * Width + x) * 4;
                int argb = ((data[index + 3] | 0xFF) << 24)
                + ((data[index] | 0xFF) << 16)
                + ((data[index + 1] | 0xFF) << 8)
                + (data[index + 2] | 0xFF);

                color = Color.FromArgb(argb);
                return true;
            }

            color = default;
            return false;
        }

        public byte[] GetData() => data;

        public void Clear()
        {
            for (int i = 0; i < zbuffer.Length; i++)
            {
                zbuffer[i] = 1f;
                data[i*4] = 0;
                data[i*4+1] = 0;
                data[i*4+2] = 0;
                data[i*4+3] = 0;
            }
        }

        internal void SetPixel(Vector3iif v, Color value)
        {
            if (-1 < v.X && v.X < Width && -1 < v.Y && v.Y < Height)
            {
                int pixelIndex = (v.Y * Width + v.X);
                if (v.Z > zbuffer[pixelIndex]) return;

                zbuffer[pixelIndex] = v.Z;
                int index = pixelIndex * 4;
                data[index] = value.R;
                data[index + 1] = value.G;
                data[index + 2] = value.B;
                data[index + 3] = value.A;
            }
        }
    }
}
