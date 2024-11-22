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
            Vector4 ambientColor = new Vector4(0.5f, 0.6f, 0.7f, 1f);

            IShaderProgram<UnlitFragmentData> litColorShader = new LitShaderProgram(Vector4.One, ambientColor);

            SceneNode scene = new SceneNode();

            if (args.Length == 0)
            {
                // default scene

                Buffer<Vector4> headTexture = BufferReader.ReadFromFile("african_head_diffuse.png");
                IShaderProgram<UnlitFragmentData> litHeadTextureShader = new LitShaderProgram(headTexture, ambientColor);
                Mesh headMesh = ObjReader.ReadFromFile("african_head.obj", calculateNormals: false);
                Transform headTransform = new Transform(Vector3.Zero, Quaternion.Identity);
                SceneNode head = new SceneNode(headTransform, scene, [new ShadedMeshRenderer<UnlitFragmentData>(headMesh, litHeadTextureShader)]);

                Buffer<Vector4> barrelTexture = BufferReader.ReadFromFile("Barrel_diffuse.png");
                IShaderProgram<UnlitFragmentData> litBarrelTextureShader = new LitShaderProgram(barrelTexture, ambientColor);
                Mesh barrelMesh = ObjReader.ReadFromFile("RustyBarrel.obj", calculateNormals: true);
                Transform barrelTransform = new Transform(new Vector3(2f, 0f, 0f), Quaternion.Identity);
                SceneNode barrel = new SceneNode(barrelTransform, scene, [new ShadedMeshRenderer<UnlitFragmentData>(barrelMesh, litBarrelTextureShader)]);

                IShaderProgram<UnlitFragmentData> unlitColorShader = new UnlitShaderProgram(Vector4.One);
                Mesh quadMesh = ObjReader.ReadFromFile("Quad.obj", calculateNormals: true);
                Transform quadTransform = new Transform(new Vector3(0f, -1.1f, 0f), EulerAngles.EulerToQuaternion(0.25f * MathF.PI, -0.5f * MathF.PI, 0f));
                SceneNode quad = new SceneNode(quadTransform, scene, [new ShadedMeshRenderer<UnlitFragmentData>(quadMesh, unlitColorShader)]);
            }
            else
            {
                // Autogenerate scene
                int nodeIndex = 0;
                foreach (string meshPath in args)
                {
                    Transform transform = new Transform(new Vector3(5f * nodeIndex, 0f, 0f), EulerAngles.EulerToQuaternion(new Vector3(0f, 0f, 0.25f * nodeIndex * MathF.PI)));

                    Mesh mesh = ObjReader.ReadFromFile(meshPath, calculateNormals: true);
                    IRenderer[] renderers =
                    [
                        new ShadedMeshRenderer<UnlitFragmentData>(mesh, litColorShader),
                    ];

                    SceneNode meshNode = new SceneNode(transform, scene, renderers);
                    nodeIndex++;
                }
            }

            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.ClientSize = new OpenTK.Mathematics.Vector2i(WindowWidth, WindowHeight);

            Camera camera = Camera.CreatePerspective(new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity), (float)BufferWidth / BufferHeight, (float)(0.25 * Math.PI), 0.1f, 100f);

            Engine engine = new Engine();

            RenderWindow renderWindow = new RenderWindow(GameWindowSettings.Default, settings, BufferWidth, BufferHeight, engine, scene, camera, ambientColor);
            renderWindow.Run();
        }
    }
}
