using System.Numerics;

namespace CpuRenderer3D
{
    public record struct Triangle(int First, int Second, int Third);
    public record struct Edge(int FirstVertex, int SecondVertex);

    public class Mesh
    {
        private Vector3[] vertices;
        private Triangle[] triangles;

        public Mesh(Vector3[] vertices, Triangle[] triangleIndexes)
        {
            this.vertices = vertices;
            this.triangles = new Triangle[triangleIndexes.Length];

            for (int i = 0; i < triangleIndexes.Length; i++)
                triangles[i] = new Triangle(triangleIndexes[i].First, triangleIndexes[i].Second, triangleIndexes[i].Third);
        }

        public Vector3[] GetVertices() => vertices;

        public Triangle[] GetTriangles()
        {
            return triangles;
        }

        public Vector3 GetVertex(int index)
        {
            return vertices[index];
        }

        public Edge[] GetEdges()
        {
            Edge[] edges = new Edge[triangles.Length * 3];
            for (int i = 0; i < triangles.Length; i++)
            {
                Triangle t = triangles[i];
                edges[i * 3] = new Edge(t.First, t.Second);
                edges[i * 3 + 1] = new Edge(t.Second, t.Third);
                edges[i * 3 + 2] = new Edge(t.Third, t.First);
            }

            //var r = edges.Distinct().ToArray();
            return edges;
        }
    }
}
