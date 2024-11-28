using System.Globalization;
using System.Numerics;

namespace DoubleY.CpuRenderer3D.Demo
{
    public static class ObjReader
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        private record struct FaceComponent(int VertexIndex, int TexCoordIndex, int NormalIndex);
        private record struct Face(FaceComponent V0, FaceComponent V1, FaceComponent V2);

        public static Mesh ReadFromFile(string filePath, bool calculateNormals)
        {
            using StreamReader streamReader = File.OpenText(filePath);
            return Read(streamReader, calculateNormals);
        }

        public static Mesh Read(StreamReader reader, bool calculateNormals)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> texCoords = new List<Vector2>();
            List<Face> faces = new List<Face>();

            int lineIndex = 0;
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                int commentStartIndex = line.IndexOf('#');
                if (commentStartIndex != -1)
                    line = line[..commentStartIndex];

                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] lineParts = line.Split(" ", StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                if (lineParts.Length == 0) continue;

                switch (lineParts[0])
                {
                    case "v":
                    {
                        if (lineParts.Length < 4) throw new ArgumentException($"Bad vertex compoment number in line {lineIndex}");

                        if (!float.TryParse(lineParts[1], Culture, out float x)) throw new ArgumentException($"Bad vertex.X value in line {lineIndex}");
                        if (!float.TryParse(lineParts[2], Culture, out float y)) throw new ArgumentException($"Bad vertex.Y value in line {lineIndex}");
                        if (!float.TryParse(lineParts[3], Culture, out float z)) throw new ArgumentException($"Bad vertex.Z value in line {lineIndex}");

                        vertices.Add(new Vector3(x, y, z));
                        break;
                    }
                    case "vn":
                    {
                        if (lineParts.Length < 4) throw new ArgumentException($"Bad vertex normal compoment number in line {lineIndex}");

                        if (!float.TryParse(lineParts[1], Culture, out float x)) throw new ArgumentException($"Bad normal.X value in line {lineIndex}");
                        if (!float.TryParse(lineParts[2], Culture, out float y)) throw new ArgumentException($"Bad normal.Y value in line {lineIndex}");
                        if (!float.TryParse(lineParts[3], Culture, out float z)) throw new ArgumentException($"Bad normal.Z value in line {lineIndex}");

                        normals.Add(new Vector3(x, y, z));
                        break;
                    }
                    case "vt":
                    {
                        if (lineParts.Length < 3) throw new ArgumentException($"Bad texture coord compoment number in line {lineIndex}");

                        if (!float.TryParse(lineParts[1], Culture, out float u)) throw new ArgumentException($"Bad texture.U value in line {lineIndex}");
                        if (!float.TryParse(lineParts[2], Culture, out float v)) throw new ArgumentException($"Bad texture.V value in line {lineIndex}");

                        texCoords.Add(new Vector2(u, v));
                        break;
                    }
                    case "f":
                    {
                        if (lineParts.Length < 4) throw new ArgumentException($"Bad face compoment number in line {lineIndex}");

                        string[] faceComponent0Str = lineParts[1].Split("/", StringSplitOptions.TrimEntries);
                        if (faceComponent0Str.Length == 0) throw new ArgumentException($"Bad face component {0} indices number in line {lineIndex}");

                        int faceV0texCoordIndex = 0;
                        int faceV0NormalIndex = 0;
                        if (!int.TryParse(faceComponent0Str[0], Culture, out int faceV0vertexIndex)) throw new ArgumentException($"Bad face component {0} vertex index in line {lineIndex}");
                        if (faceComponent0Str.Length > 1 && !string.IsNullOrWhiteSpace(faceComponent0Str[1]) && !int.TryParse(faceComponent0Str[1], Culture, out faceV0texCoordIndex)) throw new ArgumentException($"Bad face component {0} texure coord index in line {lineIndex}");
                        if (faceComponent0Str.Length > 2 && !string.IsNullOrWhiteSpace(faceComponent0Str[2]) && !int.TryParse(faceComponent0Str[2], Culture, out faceV0NormalIndex)) throw new ArgumentException($"Bad face component {0} normal index in line {lineIndex}");
                        FaceComponent faceComponent0 = new FaceComponent(faceV0vertexIndex, faceV0texCoordIndex, faceV0NormalIndex);

                        string[] faceComponent1Str = lineParts[2].Split("/", StringSplitOptions.TrimEntries);
                        if (faceComponent1Str.Length == 0) throw new ArgumentException($"Bad face component {1} indices number in line {lineIndex}");

                        int faceV1texCoordIndex = 0;
                        int faceV1NormalIndex = 0;
                        if (!int.TryParse(faceComponent1Str[0], Culture, out int faceV1vertexIndex)) throw new ArgumentException($"Bad face component {1} vertex index in line {lineIndex}");
                        if (faceComponent1Str.Length > 1 && !string.IsNullOrWhiteSpace(faceComponent1Str[1]) && !int.TryParse(faceComponent1Str[1], Culture, out faceV1texCoordIndex)) throw new ArgumentException($"Bad face component {1} texure coord index in line {lineIndex}");
                        if (faceComponent1Str.Length > 2 && !string.IsNullOrWhiteSpace(faceComponent1Str[2]) && !int.TryParse(faceComponent1Str[2], Culture, out faceV1NormalIndex)) throw new ArgumentException($"Bad face component {1} normal index in line {lineIndex}");
                        FaceComponent faceComponent1 = new FaceComponent(faceV1vertexIndex, faceV1texCoordIndex, faceV1NormalIndex);

                        string[] faceComponent2Str = lineParts[3].Split("/", StringSplitOptions.TrimEntries);
                        if (faceComponent2Str.Length == 0) throw new ArgumentException($"Bad face component {2} indices number in line {lineIndex}");

                        int faceV2texCoordIndex = 0;
                        int faceV2NormalIndex = 0;
                        if (!int.TryParse(faceComponent2Str[0], Culture, out int faceV2vertexIndex)) throw new ArgumentException($"Bad face component {2} vertex index in line {lineIndex}");
                        if (faceComponent2Str.Length > 1 && !string.IsNullOrWhiteSpace(faceComponent2Str[1]) && !int.TryParse(faceComponent2Str[1], Culture, out faceV2texCoordIndex)) throw new ArgumentException($"Bad face component {2} texure coord index in line {lineIndex}");
                        if (faceComponent2Str.Length > 2 && !string.IsNullOrWhiteSpace(faceComponent2Str[2]) && !int.TryParse(faceComponent2Str[2], Culture, out faceV2NormalIndex)) throw new ArgumentException($"Bad face component {2} normal index in line {lineIndex}");
                        FaceComponent faceComponent2 = new FaceComponent(faceV2vertexIndex, faceV2texCoordIndex, faceV2NormalIndex);

                        faces.Add(new Face(faceComponent0, faceComponent1, faceComponent2));
                        break;
                    }
                }
                ++lineIndex;
            }

            Vector3[] verticesArray = vertices.ToArray();
            Vector2[] texCoordsArray = texCoords.ToArray();
            Vector3[] normalsArray = normals.ToArray();

            Triangle[] trianglesArray = faces.Select(f =>
                    new Triangle(
                            new TriangleVertex(f.V0.VertexIndex - 1, f.V0.NormalIndex - 1, f.V0.TexCoordIndex - 1),
                            new TriangleVertex(f.V1.VertexIndex - 1, f.V1.NormalIndex - 1, f.V1.TexCoordIndex - 1),
                            new TriangleVertex(f.V2.VertexIndex - 1, f.V2.NormalIndex - 1, f.V2.TexCoordIndex - 1))).ToArray();

            if (calculateNormals)
            {
                Mesh.CalculateNormals(verticesArray, trianglesArray, out normalsArray, out trianglesArray);
                return Mesh.Create(verticesArray, normalsArray, texCoordsArray, trianglesArray);
            }
            else
            {
                return Mesh.Create(verticesArray, normalsArray, texCoordsArray, trianglesArray);
            }
        }
    }
}
