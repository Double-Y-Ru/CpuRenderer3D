using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CpuRenderer3D
{
    //public record struct Edge(int V0, int V1, int T0, int T1);
    public record struct Triangle(int V0, int V1, int V2);
    public record struct Edge(int V0, int V1, int[] Tris);

    public class Mesh
    {
        private readonly Vector3[] _vertices;
        private readonly Triangle[] _triangles;
        private readonly Edge[] _edges;

        private Mesh(Vector3[] vertices, Triangle[] triangles, Edge[] edges)
        {
            _vertices = vertices;
            _triangles = triangles;
            _edges = edges;
        }

        public Vector3[] GetVertices() => _vertices;
        public Triangle[] GetTriangles() => _triangles;
        public Edge[] GetEdges() => _edges;

        public Vector3 GetVertex(int index)
        {
            return _vertices[index];
        }

        public static Mesh Create(Vector3[] vertices, Triangle[] triangles)
        {
            Dictionary<EdgeKey, HashSet<int>> edgeTrisMap = new Dictionary<EdgeKey, HashSet<int>>(new BareEdgeComparer());

            for (int tid = 0; tid < triangles.Length; tid++)
            {
                Triangle tri = triangles[tid];

                AccumulateEdgeTriangle(new EdgeKey(tri.V0, tri.V1), tid);
                AccumulateEdgeTriangle(new EdgeKey(tri.V1, tri.V2), tid);
                AccumulateEdgeTriangle(new EdgeKey(tri.V2, tri.V0), tid);
            }

            Edge[] edges = edgeTrisMap.Select(edgeTris => new Edge(edgeTris.Key.V0, edgeTris.Key.V1, edgeTris.Value.ToArray())).ToArray();

            return new Mesh(vertices, triangles, edges);

            void AccumulateEdgeTriangle(EdgeKey edge, int triangleId)
            {
                if (edgeTrisMap.TryGetValue(edge, out HashSet<int>? edgeTris))
                    edgeTris.Add(triangleId);
                else
                    edgeTrisMap[edge] = new HashSet<int>() { triangleId };
            }
        }

        public readonly record struct EdgeKey
        {
            public readonly int V0;
            public readonly int V1;

            public EdgeKey(int v0, int v1)
            {
                bool isAscendingOrder = v0 < v1;

                V0 = isAscendingOrder ? v0 : v1;
                V1 = isAscendingOrder ? v1 : v0;
            }
        }

        private class BareEdgeComparer : IEqualityComparer<EdgeKey>
        {
            public bool Equals(EdgeKey a, EdgeKey b) => a.V0 == b.V0 && a.V1 == b.V1;
            public int GetHashCode([DisallowNull] EdgeKey edge) => HashCode.Combine(edge.V0, edge.V1);
        }
    }
}
