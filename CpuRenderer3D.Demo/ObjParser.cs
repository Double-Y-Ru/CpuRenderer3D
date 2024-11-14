using System.Numerics;

namespace CpuRenderer3D.Demo
{
    public static class ObjParser
    {
        public static Mesh Parse(string objFileText)
        {
            string[] separated = objFileText.Split("\r\n").Where(s => !string.IsNullOrEmpty(s)).ToArray();

            List<int> vertIndexes = new List<int>();
            List<int> edgIndexes = new List<int>();
            char first;
            char second;
            for (int i = 0; i < separated.Length; i++)
            {
                first = separated[i][0];
                switch (first)
                {
                    case 'v':
                        second = separated[i][1];
                        if (second != ' ')
                        {
                            separated[i] = "";
                            continue;
                        }

                        vertIndexes.Add(i);
                        separated[i] = separated[i].Substring(2);
                        break;

                    case 'f':
                        edgIndexes.Add(i);
                        separated[i] = separated[i].Substring(2);
                        break;
                }
            }

            Vector3[] vertices = new Vector3[vertIndexes.Count];
            int counter = 0;
            float[] vertex = new float[3];
            foreach (int i in vertIndexes)
            {
                vertex = separated[i].Split(" ")
                    .Select(s => float.Parse(s, System.Globalization.CultureInfo.InvariantCulture)).ToArray();
                vertices[counter] = new Vector3(vertex[0], vertex[1], vertex[2]);
                counter++;
            }

            counter = 0;
            Triangle[] triangles = new Triangle[edgIndexes.Count];
            foreach (int i in edgIndexes)
            {
                int[] vertexIndices = separated[i].Split(" ").Select(a => int.Parse(a.Substring(0, a.IndexOf('/')))).ToArray();
                triangles[counter] = new Triangle(vertexIndices[0] - 1, vertexIndices[1] - 1, vertexIndices[2] - 1);
                counter++;
            }

            return new Mesh(vertices, triangles);
        }
    }
}
