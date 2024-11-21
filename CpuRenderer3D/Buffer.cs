using System;

namespace CpuRenderer3D
{
    public class Buffer<T>
    {
        public readonly T DefaultDataValue;
        public readonly int Width;
        public readonly int Height;

        private readonly T[] _data;

        public Buffer(int width, int height, T defaultValue)
        {
            Width = width;
            Height = height;
            DefaultDataValue = defaultValue;

            _data = new T[Width * Height];
            Clear();
        }

        public T Get(int index)
        {
            if (0 <= index && index < Width * Height)
                return _data[index];

            throw new IndexOutOfRangeException();
        }

        public void Set(int index, T value)
        {
            if (0 <= index && index < Width * Height)
                _data[index] = value;
            else throw new IndexOutOfRangeException();

        }

        public void Set(int x, int y, T value)
        {
            if (-1 < x && x < Width && -1 < y && y < Height)
            {
                int index = (y * Width + x);
                _data[index] = value;
            }
            else throw new IndexOutOfRangeException();
        }

        public bool TryGet(int x, int y, out T result)
        {
            if (-1 < x && x < Width && -1 < y && y < Height)
            {
                int index = (y * Width + x);

                result = _data[index];
                return true;
            }

            result = DefaultDataValue;
            return false;
        }

        public T Get(int x, int y)
        {
            if (-1 < x && x < Width && -1 < y && y < Height)
            {
                int index = (y * Width + x);

                return _data[index];
            }

            throw new IndexOutOfRangeException();
        }

        public T Sample(float u, float v)
        {
            u = Math.Clamp(u, 0f, 1f);
            v = Math.Clamp(v, 0f, 1f);

            int x = (int)MathF.Round(u * (Width - 1));
            int y = Height - 1 - (int)MathF.Round(v * (Height - 1));

            return Get(x, y);
        }

        public T[] GetData()
        {
            T[] result = new T[Width * Height];
            _data.CopyTo(result, 0);
            return result;
        }

        public void SetData(T[] data)
        {
            if (data.Length > Width * Height) throw new IndexOutOfRangeException();
            data.CopyTo(_data, 0);
        }

        public void Clear()
        {
            for (int i = 0; i < _data.Length; i++)
                _data[i] = DefaultDataValue;
        }
    }
}
