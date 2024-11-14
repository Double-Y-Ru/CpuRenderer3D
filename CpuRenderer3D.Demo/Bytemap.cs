using System.Drawing;

namespace CpuRenderer3D.Demo
{
    public class Bytemap
    {
        public readonly int Width;
        public readonly int Height;

        private readonly byte[] _data;
        private readonly float[] _zbuffer;

        public Bytemap(int width, int height)
        {
            Width = width;
            Height = height;
            _data = new byte[width * height * 4];

            _zbuffer = new float[width * height];
            for (int i = 0; i < _zbuffer.Length; i++)
                _zbuffer[i] = 1f;
        }

        public bool TryGetPixel(int x, int y, out Color color)
        {
            if (-1 < x && x < Width && -1 < y && y < Height)
            {
                int index = (y * Width + x) * 4;
                int argb = ((_data[index + 3] | 0xFF) << 24)
                + ((_data[index] | 0xFF) << 16)
                + ((_data[index + 1] | 0xFF) << 8)
                + (_data[index + 2] | 0xFF);

                color = Color.FromArgb(argb);
                return true;
            }

            color = default;
            return false;
        }

        public byte[] GetData() => _data;

        public void Clear()
        {
            for (int i = 0; i < _zbuffer.Length; i++)
            {
                _zbuffer[i] = 1f;
                _data[i * 4] = 0;
                _data[i * 4 + 1] = 0;
                _data[i * 4 + 2] = 0;
                _data[i * 4 + 3] = 0;
            }
        }

        internal void SetPixel(Vector3iif v, Color value)
        {
            if (-1 < v.X && v.X < Width && -1 < v.Y && v.Y < Height)
            {
                int pixelIndex = v.Y * Width + v.X;
                if (v.Z > _zbuffer[pixelIndex]) return;

                _zbuffer[pixelIndex] = v.Z;
                int index = pixelIndex * 4;
                _data[index] = value.R;
                _data[index + 1] = value.G;
                _data[index + 2] = value.B;
                _data[index + 3] = value.A;
            }
        }
    }
}
