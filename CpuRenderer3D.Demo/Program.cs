using System.Numerics;
using OpenTK.Windowing.Desktop;

namespace CpuRenderer3D.Demo
{
    internal class Program
    {
        const int WindowWidth = 512;
        const int WindowHeight = 384;

        const int BufferWidth = 1024;
        const int BufferHeight = 768;

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

            CpuRendererAdapter cpuRenderer = new CpuRendererAdapter(new CpuRenderer());

            RenderWindow renderWindow = new RenderWindow(GameWindowSettings.Default, settings, BufferWidth, BufferHeight, cpuRenderer, entities, camera);
            renderWindow.Run();

            CpuRendererLegacy cpuRendererLegacy = new CpuRendererLegacy();

            RenderWindow renderWindowLegacy = new RenderWindow(GameWindowSettings.Default, settings, BufferWidth, BufferHeight, cpuRendererLegacy, entities, camera);
            renderWindowLegacy.Run();
        }
    }
}
