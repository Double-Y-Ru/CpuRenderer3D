using System.Numerics;
using OpenTK.Windowing.Desktop;

namespace CpuRenderer3D.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Mesh tooth13 = ObjParser.Parse(File.ReadAllText(@"C:\Games\tooth_13.obj"));
            //Mesh tooth14 = ObjParser.Parse(File.ReadAllText(@"C:\Games\tooth_14.obj"));

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0,3,0), new Vector3(-2,0,0), new Vector3(3,0,0), new Vector3(2,3,-3), new Vector3(-1,-2,2), new Vector3(2,-1,-1), new Vector3(-2,3,3), new Vector3(-3,-1,1), new Vector3(1,-2,-2)
                //new Vector3(1,1,0.5f), new Vector3(-1,1,0.5f), new Vector3(-1,-1,0.5f)
            };

            Triangle[] triangleIndexes = new Triangle[]
            {
                new Triangle(0,1,2), new Triangle(3,4,5), new Triangle(6,7,8)
                //new Triangle(0,1,2)
            };

            Mesh mesh = new Mesh(vertices, triangleIndexes);

            /*Entity[] entities = new Entity[2];
            entities[0] = new Entity(new Transform(new Vector3(5f, 0f, 0f), Quaternion.Identity), tooth13);
            entities[1] = new Entity(new Transform(new Vector3(-5f, 0f, 0f), Quaternion.Identity), tooth14);*/

            Entity[] entities = new Entity[1];
            entities[0] = new Entity(new Transform(), mesh);

            ShaderGL shaderGl = new ShaderGL(
                System.Text.Encoding.Default.GetString(Resource.vertShader),
                System.Text.Encoding.Default.GetString(Resource.fragShader));

            int width = 800;
            int height = 600;
            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.ClientSize = new OpenTK.Mathematics.Vector2i(1024, 768);

            Transform camera = new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity);

            float aspect = (float)width / height;
            float nearPlane = 0.1f;
            float farPlane = 200;

            Matrix4x4 screenScale = new Matrix4x4(
                0.5f * width, 0, 0, 0,
                0, 0.5f * height, 0, 0,
                0, 0, 1, 0,
                0.5f * width, 0.5f * height, 0, 1);

            Matrix4x4.Invert(camera.GetMatrix(), out Matrix4x4 worldView);
            RenderingContext renderingContext = new RenderingContext(
                colorBuffer: new Buffer<Vector4>(width, height, Vector4.Zero),
                zBuffer: new Buffer<float>(width, height, 1f),
                worldView: worldView,
                viewProjection: Matrix4x4.CreatePerspectiveFieldOfView((float)(0.5f * Math.PI), aspect, nearPlane, farPlane),
                clipView: screenScale);

            IShaderProgram shaderProgram = new UnlitShaderProgram();
            CpuRenderer cpuRenderer = new CpuRenderer();

            WindowRender windowRender = new WindowRender(GameWindowSettings.Default, settings, shaderGl,
                cpuRenderer, entities, shaderProgram, renderingContext);
            windowRender.Run();

            CpuRendererLegacy cpuRendererLegacy = new CpuRendererLegacy();
            WindowRenderLegacy windowRenderLegacy = new WindowRenderLegacy(GameWindowSettings.Default, settings, shaderGl,
                cpuRendererLegacy, entities, camera);
            windowRenderLegacy.Run();
        }
    }
}
