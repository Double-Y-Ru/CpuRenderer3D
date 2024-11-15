using System.Numerics;
using OpenTK.Windowing.Desktop;

namespace CpuRenderer3D.Demo
{
    internal class Program
    {
        const int WindowWidth = 1024;
        const int WindowHeight = 768;

        const int BufferWidth = 512;
        const int BufferHeight = 384;

        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            List<Entity> entities = new List<Entity>();

            IShaderProgram shaderProgram = new UnlitShaderProgram();

            foreach (string meshPath in args)
            {
                Mesh mesh = ObjParser.Parse(File.ReadAllText(meshPath));
                entities.Add(new Entity(new Transform(), mesh, shaderProgram));
            }

            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.ClientSize = new OpenTK.Mathematics.Vector2i(WindowWidth, WindowHeight);

            Camera camera = Camera.CreatePerspective(new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity), (float)BufferWidth / BufferHeight, (float)(0.5 * Math.PI), 0.1f, 200f);

            CpuRenderer cpuRenderer = new CpuRenderer();

            RenderWindow renderWindow = new RenderWindow(GameWindowSettings.Default, settings, BufferWidth, BufferHeight, cpuRenderer, entities, camera);
            renderWindow.Run();

            CpuRendererLegacy cpuRendererLegacy = new CpuRendererLegacy();

            RenderWindowLegacy renderWindowLegacy = new RenderWindowLegacy(GameWindowSettings.Default, settings, cpuRendererLegacy, entities, camera, BufferWidth, BufferHeight);
            renderWindowLegacy.Run();
        }
    }
}
