#nullable disable

using System.Numerics;

namespace CpuRenderer3D
{
    public class RenderingContext
    {
        public readonly Buffer<Vector4> ColorBuffer;
        public readonly Buffer<float> ZBuffer;

        public Matrix4x4 ModelWorld;// entity
        public Matrix4x4 WorldView;// camera
        public Matrix4x4 ViewProjection;// projection
        public Matrix4x4 ProjectionClip;// rendering screen

        public Matrix4x4 WorldProjection;// WorldView * ViewProjection 
        public Matrix4x4 ModelProjection;// ModelWorld * ModelProjection 
        public Matrix4x4 ModelClip;// ProjectionClip * ClipView

        public RenderingContext(Buffer<Vector4> colorBuffer, Buffer<float> zBuffer,
            Matrix4x4 worldView, Matrix4x4 viewProjection, Matrix4x4 projectionClip)
        {
            ColorBuffer = colorBuffer;
            ZBuffer = zBuffer;

            WorldView = worldView;
            ViewProjection = viewProjection;
            ProjectionClip = projectionClip;

            ModelWorld = Matrix4x4.Identity; //it will be set personally for each entity in rendering cycle

            WorldProjection = WorldView * ViewProjection;
            ModelProjection = ModelWorld * WorldProjection;
            ModelClip = ProjectionClip * ModelProjection;
        }

        public void SetWorldView(Matrix4x4 camera)
        {
            WorldView = camera;
            ModelProjection = ModelWorld * WorldProjection;
            ModelClip = ProjectionClip * ModelProjection;
        }

        public void SetModelWorld(Matrix4x4 modelWorld)
        {
            ModelWorld = modelWorld;
            ModelProjection = ModelWorld * WorldProjection;
            ModelClip = ProjectionClip * ModelProjection;
        }
    }
}
