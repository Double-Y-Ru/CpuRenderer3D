using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class RenderWindow : GameWindow
    {
        private readonly CpuRenderer _renderer;
        private readonly IReadOnlyList<Entity> _entities;
        private readonly Camera _camera;
        private readonly Buffer<Vector4> _colorBuffer;
        private readonly Buffer<float> _depthBuffer;

        private readonly float[] _vertices =
        {
            //Position    | Texture coordinates
             1f,  1f, 0.0f, 1.0f, 1.0f,
             1f, -1f, 0.0f, 1.0f, 0.0f,
            -1f, -1f, 0.0f, 0.0f, 0.0f,
            -1f,  1f, 0.0f, 0.0f, 1.0f
        };

        private readonly uint[] _indices =
        {
            0, 1, 2,
            0, 2, 3
        };

        private int EBO;
        private int VOB;
        private int VAO;

        private ShaderGL? _shaderGl;
        private Texture? _texture;

        private bool _dirty = true;

        public RenderWindow(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, int bufferWidth, int bufferHeight,
            CpuRenderer renderer, IReadOnlyList<Entity> entities, Camera camera)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _renderer = renderer;
            _entities = entities;
            _camera = camera;
            _colorBuffer = new Buffer<Vector4>(bufferWidth, bufferHeight, Vector4.Zero);
            _depthBuffer = new Buffer<float>(bufferWidth, bufferHeight, 1f);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _shaderGl = new ShaderGL(
                System.Text.Encoding.Default.GetString(Resource.vertShader),
                System.Text.Encoding.Default.GetString(Resource.fragShader));

            _texture = new Texture(_colorBuffer.GetData(), _colorBuffer.Width, _colorBuffer.Height);

            VOB = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VOB);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.DynamicDraw);

            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.DynamicDraw);

            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            int texCoordLocation = _shaderGl.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        protected override void OnUnload()
        {
            _shaderGl!.Dispose();

            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            _dirty |= TryMoveCamera(_camera);
            _dirty |= RotateCamera(_camera);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (!_dirty) return;

            base.OnRenderFrame(e);

            _renderer.Render(_entities, _camera, _colorBuffer, _depthBuffer);
            _texture!.Replace(_colorBuffer.GetData(), _colorBuffer.Width, _colorBuffer.Height);
            _shaderGl!.Use();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            _colorBuffer.Clear();
            _depthBuffer.Clear();
            SwapBuffers();

            _dirty = false;
        }

        private bool TryMoveCamera(Camera camera)
        {
            const float cameraRegularSpeedMetersPerFrame = 0.02f;
            const float cameraHighSpeedMetersPerFrame = 0.2f;

            Vector3 cameraMovementDirectionLocal = Vector3.Zero;

            cameraMovementDirectionLocal.Z += KeyboardState.IsKeyDown(Keys.S) ? 1f : 0f;
            cameraMovementDirectionLocal.Z -= KeyboardState.IsKeyDown(Keys.W) ? 1f : 0f;

            cameraMovementDirectionLocal.X += KeyboardState.IsKeyDown(Keys.D) ? 1f : 0f;
            cameraMovementDirectionLocal.X -= KeyboardState.IsKeyDown(Keys.A) ? 1f : 0f;

            cameraMovementDirectionLocal.Y += KeyboardState.IsKeyDown(Keys.E) ? 1f : 0f;
            cameraMovementDirectionLocal.Y -= KeyboardState.IsKeyDown(Keys.Q) ? 1f : 0f;

            if (cameraMovementDirectionLocal.Equals(Vector3.Zero)) return false;

            float speed = KeyboardState.IsKeyDown(Keys.LeftShift) ? cameraHighSpeedMetersPerFrame : cameraRegularSpeedMetersPerFrame;

            camera.Transform.Origin += Vector3.Transform(cameraMovementDirectionLocal * speed, camera.Transform.Rotation);

            return true;
        }

        private bool RotateCamera(Camera camera)
        {
            const float cameraRotationRadsPerPixel = 0.002f;

            if (!MouseState.IsButtonDown(MouseButton.Right)) return false;

            Vector3 cameraEulerRotationDeltaPx = new Vector3(-MouseState.Delta.Y, -MouseState.Delta.X, 0f);

            if (cameraEulerRotationDeltaPx.Equals(Vector3.Zero)) return false;

            Vector3 cameraEulerRotation = EulerAngles.QuaternionToEuler(camera.Transform.Rotation);
            camera.Transform.Rotation = EulerAngles.EulerToQuaternion(cameraEulerRotation + cameraEulerRotationDeltaPx * cameraRotationRadsPerPixel);

            return true;
        }
    }
}
