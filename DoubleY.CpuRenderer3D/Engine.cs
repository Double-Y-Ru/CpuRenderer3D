using System.Numerics;

namespace DoubleY.CpuRenderer3D
{
    public class Engine
    {
        private readonly RenderingContext _renderingContext;
        private readonly SceneNode _scene;
        private readonly Camera _camera;

        public Engine(SceneNode scene, Camera camera, int bufferWidth, int bufferHeight, Vector4 backgroundColor)
        {
            _renderingContext = new RenderingContext(
                new Buffer<Vector4>(bufferWidth, bufferHeight, backgroundColor),
                new Buffer<float>(bufferWidth, bufferHeight, 1f),
                worldView: camera.GetWorldViewMatrix(),
                viewClip: camera.GetViewProjectionMatrix(),
                clipScreen: CreateClipScreenMatrix(bufferWidth, bufferHeight));

            _scene = scene;
            _camera = camera;
        }

        public void Render(out Buffer<Vector4> colorBuffer, out Buffer<float> depthBuffer)
        {
            _renderingContext.SetWorldView(_camera.GetWorldViewMatrix());

            RenderRecursive(_scene, _renderingContext);

            colorBuffer = _renderingContext.ColorBuffer;
            depthBuffer = _renderingContext.DepthBuffer;
        }

        public void ClearBuffers()
        {
            _renderingContext.ColorBuffer.Clear();
            _renderingContext.DepthBuffer.Clear();
        }

        public static void Render(SceneNode scene, Camera camera, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer)
        {
            RenderingContext renderingContext = new RenderingContext(
                colorBuffer,
                depthBuffer,
                worldView: camera.GetWorldViewMatrix(),
                viewClip: camera.GetViewProjectionMatrix(),
                clipScreen: CreateClipScreenMatrix(colorBuffer.Width, colorBuffer.Height));

            RenderRecursive(scene, renderingContext);
        }

        private static void RenderRecursive(SceneNode sceneNode, RenderingContext renderingContext)
        {
            renderingContext.SetModelWorld(sceneNode.GlobalTransform.GetMatrix());

            foreach (IRenderer renderer in sceneNode.GetRenderers())
                renderer.Render(renderingContext);

            foreach (SceneNode child in sceneNode.GetChildren())
                RenderRecursive(child, renderingContext);
        }

        private static Matrix4x4 CreateClipScreenMatrix(int bufferWidth, int bufferHeight)
        {
            float halfWidth = 0.5f * bufferWidth;
            float halfHeight = 0.5f * bufferHeight;

            return new Matrix4x4(
                halfWidth, 0, 0, 0,
                0, halfHeight, 0, 0,
                0, 0, 1, 0,
                halfWidth, halfHeight, 0, 1);
        }
    }
}