﻿#nullable disable

using System.Numerics;

namespace CpuRenderer3D
{
    public class RenderingContext
    {
        public readonly Buffer<Vector4> ColorBuffer;
        public readonly Buffer<float> DepthBuffer;
        public readonly Buffer<int> DataBuffer;

        public Matrix4x4 ModelWorld;// entity
        public Matrix4x4 WorldView;// camera
        public Matrix4x4 ViewClip;// projection
        public Matrix4x4 ClipScreen;// rendering screen

        public Matrix4x4 WorldClip;// WorldView * ViewClip 
        public Matrix4x4 ModelClip;// ModelWorld * WorldClip 
        public Matrix4x4 ModelScreen;// ClipScreen * ModelClip

        public RenderingContext(Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer, Buffer<int> dataBuffer,
            Matrix4x4 worldView, Matrix4x4 viewClip, Matrix4x4 clipScreen)
        {
            ColorBuffer = colorBuffer;
            DepthBuffer = depthBuffer;
            DataBuffer = dataBuffer;

            WorldView = worldView;
            ViewClip = viewClip;
            ClipScreen = clipScreen;

            ModelWorld = Matrix4x4.Identity; //it will be set personally for each entity in rendering cycle

            WorldClip = WorldView * ViewClip;
            ModelClip = ModelWorld * WorldClip;
            ModelScreen = ClipScreen * ModelClip;
        }

        public void SetWorldView(Matrix4x4 camera)
        {
            WorldView = camera;
            ModelClip = ModelWorld * WorldClip;
            ModelScreen = ClipScreen * ModelClip;
        }

        public void SetModelWorld(Matrix4x4 modelWorld)
        {
            ModelWorld = modelWorld;
            ModelClip = ModelWorld * WorldClip;
            ModelScreen = ClipScreen * ModelClip;
        }
    }
}
