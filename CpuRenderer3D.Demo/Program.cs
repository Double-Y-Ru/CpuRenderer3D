using System.Numerics;
using CpuRenderer3D.Renderers;
using CpuRenderer3D.Shaders;
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
            Vector4 bgColor = new Vector4(0.5f, 0.6f, 0.7f, 1f);
            Vector4 ambientColor = new Vector4(0.1f, 0.12f, 0.14f, 1f);
            Vector4 lightColor = Vector4.One;

            SceneNode scene = new SceneNode();

            if (args.Length == 0)
            {
                // default scene

                Buffer<Vector4> headDiffuseTexture = BufferReader.ReadRgbaFromFile("african_head_diffuse.png");
                Buffer<float> headSpecularTexture = BufferReader.ReadGrayFromFile("african_head_spec.png");
                LitShaderProgram litHeadTextureShader = new LitShaderProgram(headDiffuseTexture, headSpecularTexture, ambientColor, lightColor);
                Mesh headMesh = ObjReader.ReadFromFile("african_head.obj", calculateNormals: false);
                Transform headTransform = new Transform(Vector3.Zero, Quaternion.Identity);
                //scene.AddChild(new SceneNode(headTransform, [new ShadedMeshRenderer<LitFragmentData>(headMesh, litHeadTextureShader, litHeadTextureShader)]));
                scene.AddChild(new SceneNode(headTransform, [new ShadedMeshWithContourRenderer<LitFragmentData>(headMesh, litHeadTextureShader, litHeadTextureShader, Vector4.One)]));

                Buffer<Vector4> barrelDiffuseTexture = BufferReader.ReadRgbaFromFile("Barrel_diffuse.png");
                Buffer<float> barrelSpecularTexture = BufferReader.ReadGrayFromFile("Barrel_spec.png");
                LitShaderProgram litBarrelTextureShader = new LitShaderProgram(barrelDiffuseTexture, barrelSpecularTexture, ambientColor, lightColor);
                Mesh barrelMesh = ObjReader.ReadFromFile("RustyBarrel.obj", calculateNormals: true);
                Transform barrelTransform = new Transform(new Vector3(2f, 0f, 0f), Quaternion.Identity);
                scene.AddChild(new SceneNode(barrelTransform, [new ShadedMeshRenderer<LitFragmentData>(barrelMesh, litBarrelTextureShader, litBarrelTextureShader)]));

                Buffer<Vector4> chessTexture = CreateChessTexture(4, 4, Vector4.One, Vector4.Zero);
                UnlitShaderProgram unlitColorShader = new UnlitShaderProgram(chessTexture);
                Mesh quadMesh = ObjReader.ReadFromFile("Quad.obj", calculateNormals: true);
                Transform quadTransform = new Transform(new Vector3(0f, -1.1f, 0f), EulerAngles.EulerToQuaternion(0.25f * MathF.PI, -0.5f * MathF.PI, 0f));
                scene.AddChild(new SceneNode(quadTransform, [new ShadedMeshRenderer<UnlitFragmentData>(quadMesh, unlitColorShader, unlitColorShader)]));
            }
            else
            {
                IShaderProgram<LitFragmentData> litColorShader = new LitShaderProgram(Buffer<Vector4>.Single(new Vector4(0.5f, 0.5f, 0.5f, 1f)), Buffer<float>.Single(0.1f), ambientColor, lightColor);

                // Autogenerate scene
                int nodeIndex = 0;
                foreach (string meshPath in args)
                {
                    Transform transform = new Transform(new Vector3(5f * nodeIndex, 0f, 0f), EulerAngles.EulerToQuaternion(new Vector3(0f, 0f, 0.25f * nodeIndex * MathF.PI)));

                    Mesh mesh = ObjReader.ReadFromFile(meshPath, calculateNormals: true);
                    IRenderer[] renderers =
                    [
                        new ColoredMeshWithContourRenderer(mesh, Vector4.One, Vector4.UnitX),
                    ];

                    scene.AddChild(new SceneNode(transform, renderers));
                    nodeIndex++;
                }
            }

            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.ClientSize = new OpenTK.Mathematics.Vector2i(WindowWidth, WindowHeight);

            Camera camera = Camera.CreatePerspective(new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity), (float)BufferWidth / BufferHeight, (float)(0.25 * Math.PI), 0.1f, 100f);

            RenderWindow renderWindow = new RenderWindow(GameWindowSettings.Default, settings, BufferWidth, BufferHeight, scene, camera, bgColor);
            renderWindow.Run();
        }

        private static Buffer<Vector4> CreateChessTexture(int width, int height, Vector4 positiveValue, Vector4 negativeValue)
        {
            Buffer<Vector4> chess = new Buffer<Vector4>(width, height, Vector4.One);

            for (int y = 0; y < height; ++y)
                for (int x = 0; x < width; ++x)
                {
                    chess.Set(x, y, (x + y) % 2 == 0 ? negativeValue : positiveValue);
                }

            return chess;
        }
    }
}
