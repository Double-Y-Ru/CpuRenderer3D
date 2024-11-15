using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public class RenderWindowLegacy : GameWindow
    {
        private readonly CpuRendererLegacy _renderer;
        private IReadOnlyList<Entity> _entities;

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
        private Transform _camera;
        private Bytemap _bytemap;

        private bool dirty = true;

        public RenderWindowLegacy(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings,
            CpuRendererLegacy renderer, IReadOnlyList<Entity> entities, Transform camera, int bufferWidth, int bufferHeight)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _renderer = renderer;
            _entities = entities;
            _camera = camera;

            _bytemap = new Bytemap(bufferWidth, bufferHeight);
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            _shaderGl = new ShaderGL(
                System.Text.Encoding.Default.GetString(Resource.vertShader),
                System.Text.Encoding.Default.GetString(Resource.fragShader));

            _texture = new Texture(_bytemap);

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
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float),
                3 * sizeof(float));
        }

        protected override void OnUnload()
        {
            _texture!.Dispose();
            _shaderGl!.Dispose();

            base.OnUnload();
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            MoveCamera();
            RotateCamera();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            if (!dirty) return;

            base.OnRenderFrame(e);

            _renderer.Render(_camera, _entities, _bytemap);
            _texture!.Replace(_bytemap);
            _shaderGl!.Use();

            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);

            SwapBuffers();

            dirty = false;
        }

        private void MoveCamera()
        {
            int dirZ = (KeyboardState.IsKeyPressed(Keys.S) ? 1 : 0) + (KeyboardState.IsKeyPressed(Keys.W) ? -1 : 0);
            int dirX = (KeyboardState.IsKeyPressed(Keys.D) ? 1 : 0) + (KeyboardState.IsKeyPressed(Keys.A) ? -1 : 0);
            int dirY = (KeyboardState.IsKeyPressed(Keys.E) ? 1 : 0) + (KeyboardState.IsKeyPressed(Keys.Q) ? -1 : 0);
            Matrix4x4 forward = Matrix4x4.CreateFromQuaternion(_camera.Rotation);
            Vector3 zMoving = Vector3.Transform(Vector3.UnitZ, forward) * dirZ;
            Vector3 xMooving = Vector3.Transform(Vector3.UnitX, forward) * dirX;
            Vector3 yMooving = Vector3.Transform(Vector3.UnitY, forward) * dirY;
            if (zMoving == Vector3.Zero &&
                xMooving == Vector3.Zero &&
                yMooving == Vector3.Zero) return;

            Vector3 dir = Vector3.Normalize(zMoving + xMooving + yMooving);
            _camera.Origin += new Vector3(dir.X, dir.Y, dir.Z);
            dirty = true;
        }

        private void RotateCamera()
        {
            int rotY = (KeyboardState.IsKeyPressed(Keys.Left) ? -1 : 0) + (KeyboardState.IsKeyPressed(Keys.Right) ? 1 : 0);
            int rotX = (KeyboardState.IsKeyPressed(Keys.Up) ? -1 : 0) + (KeyboardState.IsKeyPressed(Keys.Down) ? 1 : 0);
            Vector2 rotate = new Vector2(rotX, rotY);
            if (rotate == Vector2.Zero) return;

            const float sensitivity = 5f;
            float yawDelta = -(float)(rotate.Y / 180 * Math.PI) * sensitivity;
            float pitchDelta = -(float)(rotate.X / 180 * Math.PI) * sensitivity;
            _camera.Rotation *= Quaternion.CreateFromYawPitchRoll(yawDelta, pitchDelta, 0f);
            dirty = true;
        }
    }
}
