# About

CPU Renderer 3d is a lightweight library that allows to use 3d-scene rendering into your application. It requires no OpenGL or DirectX or Vulkan, and does not use GPU. You can embed it into any sort of application, including batch tools and server-side applications.

# How to use

To create a simple wireframe mesh render follow the steps below:

1. Create buffer objects. It is the place where the output will be drawn

```
const int BufferWidth = 600;
const int BufferHeight = 600;

Vector4 backgroundColor = new Vector4(0.5f, 0.6f, 0.7f, 1f);

Buffer<Vector4> colorBuffer = new Buffer<Vector4>(BufferWidth, BufferHeight, backgroundColor);
Buffer<float> depthBuffer = new Buffer<float>(BufferWidth, BufferHeight, 1f);
```
2. Define your camera
```
Camera camera = Camera.CreatePerspective(
  transform: new Transform(new Vector3(0f, 0f, 15f), Quaternion.Identity),
  aspect: (float)BufferWidth / BufferHeight,
  fieldOfViewRadians: (float)(0.25 * Math.PI),
  nearPlane: 0.1f,
  farPlane: 100f);
```
3. Define your object
```
Mesh mesh = YourObjReader.ReadFromFile("mesh.obj");
Vector4 wireframeColor = new Vector4(1f, 1f, 1f, 1f); // White
WireframeRenderer renderer = new WireframeRenderer(mesh, wireframeColor);
SceneNode sceneNode = new SceneNode(Transform.Identity, [renderer]);
```
4. Render the object
```
Engine.Render(sceneNode, camera, colorBuffer, depthBuffer);
```

That is all! The output data is now stored in your `colorBuffer` in RBGA float format. You can do with it whatewer you want. For example, here is the code to save the result to PNG file using SixLabors.ImageSharp library:
```
Image<RgbaVector> image = new Image<RgbaVector>(ImageWidth, ImageHeight);

for (int y = 0; y < ImageHeight; ++y)
  for (int x = 0; x < ImageWidth; ++x)
  {
    Vector4 pixelColor = colorBuffer.Get(x, y);
    image[x, y] = new RgbaVector(pixelColor.X, pixelColor.Y, pixelColor.Z, pixelColor.W);
  }

image.SaveAsPng(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), $"renderedModel.png"));
```
The output file will contain an image like that:

![cpu renderer output](https://raw.githubusercontent.com/Double-Y-Ru/CpuRenderer3D/main/TestData/cpu%20renderer%20output.png)


