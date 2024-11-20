using System.Numerics;
using OpenTK.Windowing.Desktop;

namespace CpuRenderer3D.Demo
{
    internal class Program
    {
        const int WindowWidth = 1024;
        const int WindowHeight = 768;

        const int BufferWidth = WindowWidth * 1;
        const int BufferHeight = WindowHeight * 1;

        static void Main(string[] args)
        {
            if (args.Length == 0)
                return;

            SceneNode scene = new SceneNode();

            Buffer<Vector4> texture = BufferReader.ReadFromFile("Barrel_diffuse.png");

            //IShaderProgram shaderProgram = new UnlitShaderProgram(texture); // Use only for objects wit texture coords
            IShaderProgram shaderProgram = new UnlitShaderProgram(Vector4.UnitZ);

            int nodeIndex = 0;
            foreach (string meshPath in args)
            {
                using (StreamReader streamReader = File.OpenText(meshPath))
                {
                    Transform transform = new Transform(new Vector3(5f * nodeIndex, 0f, 0f), EulerAngles.EulerToQuaternion(new Vector3(0f, 0f, 0.25f * nodeIndex * MathF.PI)));

                    Mesh mesh = ObjReader.Read(streamReader);
                    IRenderer[] renderers =
                    [
                        new MeshRenderer(mesh, shaderProgram),
                        new ContourRenderer(mesh, shaderProgram),
                    ];

                    SceneNode meshNode = new SceneNode(transform, scene, renderers);
                }
                nodeIndex++;
            }

            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.ClientSize = new OpenTK.Mathematics.Vector2i(WindowWidth, WindowHeight);

            Camera camera = Camera.CreatePerspective(new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity), (float)BufferWidth / BufferHeight, (float)(0.5 * Math.PI), 0.1f, 20f);

            CpuRenderer cpuRenderer = new CpuRenderer();

            RenderWindow renderWindow = new RenderWindow(GameWindowSettings.Default, settings, BufferWidth, BufferHeight, cpuRenderer, scene, camera);
            renderWindow.Run();
        }
    }
}
