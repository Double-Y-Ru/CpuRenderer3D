namespace CpuRenderer3D
{
    public class Buffer<T>
    {
        public readonly T DefaultDataValue;
        public readonly int Width;
        public readonly int Height;

        private readonly T[] data;

        public Buffer(int width, int height, T defaultValue)
        {
            Width = width;
            Height = height;
            this.DefaultDataValue = defaultValue;

            data = new T[Width * Height];
            Clear();
        }

        public void Set(int x, int y, T value)
        {
            if (-1 < x && x < Width && -1 < y && y < Height)
            {
                int index = (y * Width + x);

                data[index] = value;
            }
        }

        public bool TryGet(int x, int y, out T result)
        {
            if (-1 < x && x < Width && -1 < y && y < Height)
            {
                int index = (y * Width + x);

                result = data[index];
                return true;
            }

            result = DefaultDataValue;
            return false;
        }

        public T[] GetData()
        {
            T[] result = new T[Width * Height];
            data.CopyTo(result, 0);
            return result;
        }

        public void Clear()
        {
            for (int i = 0; i < data.Length; i++)
                data[i] = this.DefaultDataValue;
        }
    }
}
