using System.Numerics;

namespace CpuRenderer3D
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
            return new Camera(transform, Matrix4x4.CreatePerspectiveFieldOfView(fieldOfViewRadians, aspect, nearPlane, farPlane));
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
