using System.Numerics;

namespace CpuRenderer3D
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
                new Buffer<int>(bufferWidth, bufferHeight, 0),
                worldView: camera.GetWorldViewMatrix(),
                viewClip: camera.GetViewProjectionMatrix(),
                clipScreen: CreateProjectionClipMatrix(bufferWidth, bufferHeight));

            _scene = scene;
            _camera = camera;
        }

        public void Render(out Buffer<Vector4> colorBuffer, out Buffer<float> depthBuffer, out Buffer<int> dataBuffer)
        {
            _renderingContext.SetWorldView(_camera.GetWorldViewMatrix());

            RenderRecursive(_scene, _renderingContext);

            colorBuffer = _renderingContext.ColorBuffer;
            depthBuffer = _renderingContext.DepthBuffer;
            dataBuffer = _renderingContext.DataBuffer;
        }

        public void ClearBuffers()
        {
            _renderingContext.ColorBuffer.Clear();
            _renderingContext.DepthBuffer.Clear();
            _renderingContext.DataBuffer.Clear();
        }

        public static void Render(SceneNode scene, Camera camera, Buffer<Vector4> colorBuffer, Buffer<float> depthBuffer, Buffer<int> dataBuffer)
        {
            RenderingContext renderingContext = new RenderingContext(
                colorBuffer,
                depthBuffer,
                dataBuffer,
                worldView: camera.GetWorldViewMatrix(),
                viewClip: camera.GetViewProjectionMatrix(),
                clipScreen: CreateProjectionClipMatrix(colorBuffer.Width, colorBuffer.Height));

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

        private static Matrix4x4 CreateProjectionClipMatrix(int bufferWidth, int bufferHeight)
        {
            return new Matrix4x4(
                0.5f * bufferWidth, 0, 0, 0,
                0, 0.5f * bufferHeight, 0, 0,
                0, 0, 1, 0,
                0.5f * bufferWidth, 0.5f * bufferHeight, 0, 1);
        }
    }
}