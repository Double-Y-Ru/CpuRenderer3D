using System.Numerics;
using OpenTK.Windowing.Desktop;

namespace CpuRenderer3D.Demo
{
    internal class Program
    {
        const int WindowWidth = 1024;
        const int WindowHeight = 768;

        const int BufferWidth = 1024;
        const int BufferHeight = 768;

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

            Camera camera = new Camera(new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity), 0.1f, 200f, (float)BufferWidth / BufferHeight);

            RenderingContext renderingContext = new RenderingContext(
                colorBuffer: new Buffer<Vector4>(BufferWidth, BufferHeight, Vector4.Zero),
                zBuffer: new Buffer<float>(BufferWidth, BufferHeight, 1f),
                worldView: camera.GetWorldViewMatrix(),
                viewProjection: camera.GetViewProjectionMatrix(),
                projectionClip: Util.CreateProjectionClip(BufferWidth, BufferHeight));

            IShaderProgram shaderProgram = new UnlitShaderProgram();
            CpuRenderer cpuRenderer = new CpuRenderer();

            RenderWindow windowRender = new RenderWindow(GameWindowSettings.Default, settings,
                cpuRenderer, entities, shaderProgram, renderingContext);
            windowRender.Run();

            CpuRendererLegacy cpuRendererLegacy = new CpuRendererLegacy();
            RenderWindowLegacy windowRenderLegacy = new RenderWindowLegacy(GameWindowSettings.Default, settings,
                cpuRendererLegacy, entities, camera, BufferWidth, BufferHeight);
            windowRenderLegacy.Run();
        }
    }
}
