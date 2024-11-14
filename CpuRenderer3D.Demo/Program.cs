using System.Numerics;
using OpenTK.Windowing.Desktop;

namespace CpuRenderer3D.Demo
{
    internal class Program
    {
        const int WindowWidth = 1024;
        const int WindowHeight = 768;

        const int BufferWidth = 800;
        const int BufferHeight = 600;

        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            List<Entity> entities = new List<Entity>();

            foreach (string meshPath in args)
            {
                Mesh mesh = ObjParser.Parse(File.ReadAllText(meshPath));
                entities.Add(new Entity(new Transform(), mesh));
            }

            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.ClientSize = new OpenTK.Mathematics.Vector2i(WindowWidth, WindowHeight);

            Transform camera = new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity);

            float aspect = (float)BufferWidth / BufferHeight;
            float nearPlane = 0.1f;
            float farPlane = 200;

            Matrix4x4 screenScale = new Matrix4x4(
                0.5f * BufferWidth, 0, 0, 0,
                0, 0.5f * BufferHeight, 0, 0,
                0, 0, 1, 0,
                0.5f * BufferWidth, 0.5f * BufferHeight, 0, 1);

            Matrix4x4.Invert(camera.GetMatrix(), out Matrix4x4 worldView);
            RenderingContext renderingContext = new RenderingContext(
                colorBuffer: new Buffer<Vector4>(BufferWidth, BufferHeight, Vector4.Zero),
                zBuffer: new Buffer<float>(BufferWidth, BufferHeight, 1f),
                worldView: worldView,
                viewProjection: Matrix4x4.CreatePerspectiveFieldOfView((float)(0.5f * Math.PI), aspect, nearPlane, farPlane),
                clipView: screenScale);

            IShaderProgram shaderProgram = new UnlitShaderProgram();
            CpuRenderer cpuRenderer = new CpuRenderer();

            RenderWindow windowRender = new RenderWindow(GameWindowSettings.Default, settings,
                cpuRenderer, entities, shaderProgram, renderingContext);
            windowRender.Run();

            CpuRendererLegacy cpuRendererLegacy = new CpuRendererLegacy();
            RenderWindowLegacy windowRenderLegacy = new RenderWindowLegacy(GameWindowSettings.Default, settings,
                cpuRendererLegacy, entities, camera);
            windowRenderLegacy.Run();
        }
    }
}
