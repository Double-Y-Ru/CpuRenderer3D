using System.Numerics;
using CpuRenderer3D.Renderers;
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

            Buffer<Vector4> texture = BufferReader.ReadFromFile("african_head_diffuse.png");

            Vector4 ambientColor = new Vector4(0.5f, 0.6f, 0.7f, 1f);

            IShaderProgram<UnlitFragmentData> unlitTextureShader = new UnlitShaderProgram(texture);
            IShaderProgram<UnlitFragmentData> litTextureShader = new LitShaderProgram(texture, ambientColor);
            IShaderProgram<UnlitFragmentData> litColorShader = new LitShaderProgram(Vector4.One, ambientColor);

            int nodeIndex = 0;
            foreach (string meshPath in args)
            {
                using (StreamReader streamReader = File.OpenText(meshPath))
                {
                    Transform transform = new Transform(new Vector3(5f * nodeIndex, 0f, 0f), EulerAngles.EulerToQuaternion(new Vector3(0f, 0f, 0.25f * nodeIndex * MathF.PI)));

                    Mesh mesh = ObjReader.Read(streamReader);
                    IRenderer[] renderers =
                    [
                        new ShadedMeshRenderer<UnlitFragmentData>(mesh, litColorShader),
                    ];

                    SceneNode meshNode = new SceneNode(transform, scene, renderers);
                }
                nodeIndex++;
            }

            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.ClientSize = new OpenTK.Mathematics.Vector2i(WindowWidth, WindowHeight);

            Camera camera = Camera.CreatePerspective(new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity), (float)BufferWidth / BufferHeight, (float)(0.25 * Math.PI), 0.1f, 100f);

            Engine engine = new Engine();

            RenderWindow renderWindow = new RenderWindow(GameWindowSettings.Default, settings, BufferWidth, BufferHeight, engine, scene, camera);
            renderWindow.Run();
        }
    }
}
