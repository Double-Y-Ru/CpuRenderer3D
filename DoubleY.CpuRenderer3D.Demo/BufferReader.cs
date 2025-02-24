﻿using System.Numerics;
using StbImageSharp;

namespace DoubleY.CpuRenderer3D.Demo
{
    public static class BufferReader
    {
        public static Buffer<float> ReadGrayFromFile(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.Grey);

            Buffer<float> buffer = new Buffer<float>(image.Width, image.Height, 1f);
            byte[] data = image.Data;

            for (int i = 0; i < image.Width * image.Height; ++i)
            {
                byte v = data[i];
                float vf = v / 255f;

                buffer.Set(i, vf);
            }

            return buffer;
        }

        public static Buffer<Vector4> ReadRgbaFromFile(string filePath)
        {
            using FileStream stream = File.OpenRead(filePath);
            ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

            Buffer<Vector4> buffer = new Buffer<Vector4>(image.Width, image.Height, Vector4.One);
            byte[] data = image.Data;

            for (int i = 0; i < image.Width * image.Height; ++i)
            {
                byte r = data[i * 4];
                byte g = data[i * 4 + 1];
                byte b = data[i * 4 + 2];
                byte a = data[i * 4 + 3];

                float red = r / 255f;
                float green = g / 255f;
                float blue = b / 255f;
                float alpha = a / 255f;

                buffer.Set(i, new Vector4(red, green, blue, alpha));
            }

            return buffer;
        }
    }
}
