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
            Matrix4x4 matrixOld = CreatePerspectiveFieldOfViewStandaard(fieldOfViewRadians, aspect, nearPlane, farPlane);
            Matrix4x4 matrixNew = CreatePerspectiveFieldOfViewLegacy(fieldOfViewRadians, aspect, nearPlane, farPlane);

            return new Camera(transform, Matrix4x4.CreatePerspectiveFieldOfView(fieldOfViewRadians, aspect, nearPlane, farPlane));
        }

        public static Camera CreatePerspectiveLegacy(Transform transform, float aspect, float fieldOfViewRadians, float nearPlane, float farPlane)
        {
            return new Camera(transform, CreatePerspectiveFieldOfViewLegacy(fieldOfViewRadians, aspect, nearPlane, farPlane));
        }

        public static Matrix4x4 CreatePerspectiveFieldOfViewStandaard(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
        {
            if (fieldOfView <= 0f || fieldOfView >= (float)Math.PI)
            {
                throw new ArgumentOutOfRangeException("fieldOfView");
            }
            if (nearPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance");
            }
            if (farPlaneDistance <= 0f)
            {
                throw new ArgumentOutOfRangeException("farPlaneDistance");
            }
            if (nearPlaneDistance >= farPlaneDistance)
            {
                throw new ArgumentOutOfRangeException("nearPlaneDistance");
            }
            float yScale = 1f / MathF.Tan(fieldOfView * 0.5f);
            float xScale = yScale / aspectRatio;
            float zScale = farPlaneDistance / (nearPlaneDistance - farPlaneDistance);

            return new Matrix4x4(
                xScale, 0f, 0f, 0f,
                0f, yScale, 0f, 0f,
                0f, 0f, zScale, -1f,
                0f, 0f, nearPlaneDistance * zScale, 0f
            );
        }

        public static Matrix4x4 CreatePerspectiveFieldOfViewLegacy(float fieldOfViewRadians, float aspect, float nearPlane, float farPlane)
        {
            float yScale = 1.0f / MathF.Tan(fieldOfViewRadians / 2.0f);
            float xScale = yScale / aspect;
            float frustumLength = farPlane - nearPlane;

            return new Matrix4x4(
                xScale, 0f, 0f, 0f,
                0f, yScale, 0f, 0f,
                0f, 0f, -((farPlane + nearPlane) / frustumLength), -1f,
                0f, 0f, -(2f * nearPlane * farPlane / frustumLength), 1f
            );
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
