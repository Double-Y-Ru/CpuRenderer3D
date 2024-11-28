using System;
using System.Numerics;

namespace DoubleY.CpuRenderer3D
{
    public class Camera
    {
        public Transform Transform;

        private Matrix4x4 _viewProjectionMatrix;

        public Camera(Transform transform, Matrix4x4 viewProjectionMatrix)
        {
            Transform = transform;
            _viewProjectionMatrix = viewProjectionMatrix;
        }

        public Matrix4x4 GetWorldViewMatrix()
        {
            Matrix4x4.Invert(Transform.GetMatrix(), out Matrix4x4 worldView);
            return worldView;
        }

        public Matrix4x4 GetViewProjectionMatrix() => _viewProjectionMatrix;

        public static Camera CreatePerspective(Transform transform, float aspect, float fieldOfViewRadians, float nearPlane, float farPlane)
        {
            // return new Camera(transform, Matrix4x4.CreatePerspectiveFieldOfView(fieldOfViewRadians, aspect, nearPlane, farPlane)); // The difference is M44 == 0f

            float yScale = (1.0f / MathF.Tan(fieldOfViewRadians / 2.0f)) * aspect;
            float xScale = yScale / aspect;
            float frustumLength = farPlane - nearPlane;

            Matrix4x4 m = new Matrix4x4(
                xScale, 0, 0, 0,
                0, yScale, 0, 0,
                0, 0, -((farPlane + nearPlane) / frustumLength), -1.0f,
                0, 0, -((2.0f * nearPlane * farPlane) / frustumLength), 1f
            );

            return new Camera(transform, m);
        }

        public static Camera CreateOrthographic(Transform transform, float width, float height, float nearPlane, float farPlane)
        {
            return new Camera(transform, Matrix4x4.CreateOrthographicOffCenter(
                left: -0.5f * width,
                right: 0.5f * width,
                bottom: -0.5f * height,
                top: 0.5f * height,
                nearPlane, farPlane));
        }
    }
}
