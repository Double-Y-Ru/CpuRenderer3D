using System.Numerics;

namespace CpuRenderer3D
{
    public record struct Triangle(int V0, int V1, int V2);
    public record struct Edge(int FirstVertex, int SecondVertex);

    public class Mesh
    {
        private readonly Vector3[] _vertices;
        private readonly Triangle[] _triangles;

        public Mesh(Vector3[] vertices, Triangle[] triangleIndexes)
        {
            _vertices = vertices;
            _triangles = new Triangle[triangleIndexes.Length];

            for (int i = 0; i < triangleIndexes.Length; i++)
                _triangles[i] = new Triangle(triangleIndexes[i].V0, triangleIndexes[i].V1, triangleIndexes[i].V2);
        }

        public Vector3[] GetVertices() => _vertices;

        public Triangle[] GetTriangles()
        {
            return _triangles;
        }

        public Vector3 GetVertex(int index)
        {
            return _vertices[index];
        }

        public Edge[] GetEdges()
        {
            Edge[] edges = new Edge[_triangles.Length * 3];
            for (int i = 0; i < _triangles.Length; i++)
            {
                Triangle t = _triangles[i];
                edges[i * 3] = new Edge(t.V0, t.V1);
                edges[i * 3 + 1] = new Edge(t.V1, t.V2);
                edges[i * 3 + 2] = new Edge(t.V2, t.V0);
            }

            //var r = edges.Distinct().ToArray();
            return edges;
        }
    }
}
