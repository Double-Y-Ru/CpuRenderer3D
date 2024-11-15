using System.Numerics;

namespace CpuRenderer3D
{
    public class Camera
    {
        public Transform Transform;

        private Matrix4x4 _viewProjection;

        public Camera(Transform transform, float nearPlane, float farPlane, float aspect)
        {
            Transform = transform;
            _viewProjection = Matrix4x4.CreatePerspectiveFieldOfView((float)(0.5 * Math.PI), aspect, nearPlane, farPlane);

            //_viewProjection = Matrix4x4.CreateOrthographicOffCenter(left: -10 * aspect, right: 10 * aspect, bottom: -10, top: 10, nearPlane, farPlane);
        }

        public Matrix4x4 GetWorldViewMatrix()
        {
            Matrix4x4.Invert(Transform.GetMatrix(), out Matrix4x4 worldView);
            return worldView;
        }

        public Matrix4x4 GetViewProjectionMatrix() => _viewProjection;
    }
}
