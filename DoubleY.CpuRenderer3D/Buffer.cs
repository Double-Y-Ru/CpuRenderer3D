using System;
using System.Numerics;

namespace DoubleY.CpuRenderer3D
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

        public T Sample(Vector2 uv)
        {
            int x = (int)(uv.X * Width);
            int y = Height - 1 - (int)(uv.Y * Height);
            x = Math.Clamp(x, 0, Width - 1);
            y = Math.Clamp(y, 0, Height - 1);

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

        public static Buffer<T> Single(T value) => new Buffer<T>(1, 1, value);
    }
}
