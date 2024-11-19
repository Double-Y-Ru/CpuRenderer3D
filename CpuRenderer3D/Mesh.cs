﻿using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace CpuRenderer3D
{
    public record struct TriangleVertex(int VertexIndex, int NormalIndex, int TexCoordIndex);
    public record struct Triangle(TriangleVertex Vertex0, TriangleVertex Vertex1, TriangleVertex Vertex2);
    public record struct Edge(int Vertex0Index, int Vertex1Index, int[] Tris);

    public class Mesh
    {
        private readonly Vector3[] _vertices;
        private readonly Vector3[] _normals;
        private readonly Vector2[] _texCoords;

        private readonly Triangle[] _triangles;
        private readonly Edge[] _edges;

        private Mesh(Vector3[] vertices, Vector3[] normals, Vector2[] texCoords, Triangle[] triangles, Edge[] edges)
        {
            _vertices = vertices;
            _normals = normals;
            _texCoords = texCoords;
            _triangles = triangles;
            _edges = edges;
        }

        public Vector3[] GetVertices() => _vertices;
        public Triangle[] GetTriangles() => _triangles;
        public Edge[] GetEdges() => _edges;

        public Vector3 GetVertex(int index) => index == -1 ? Vector3.Zero : _vertices[index];
        public Vector3 GetNormal(int index) => index == -1 ? Vector3.Zero : _normals[index];
        public Vector2 GetTexCoord(int index) => index == -1 ? Vector2.Zero : _texCoords[index];

        public static Mesh Create(Vector3[] vertices, Vector3[] normals, Vector2[] texCoords, Triangle[] triangles)
        {
            Dictionary<EdgeKey, HashSet<int>> edgeTrisMap = new Dictionary<EdgeKey, HashSet<int>>(new BareEdgeComparer());

            for (int tid = 0; tid < triangles.Length; tid++)
            {
                Triangle tri = triangles[tid];

                AccumulateEdgeTriangle(new EdgeKey(tri.Vertex0.VertexIndex, tri.Vertex1.VertexIndex), tid);
                AccumulateEdgeTriangle(new EdgeKey(tri.Vertex1.VertexIndex, tri.Vertex2.VertexIndex), tid);
                AccumulateEdgeTriangle(new EdgeKey(tri.Vertex2.VertexIndex, tri.Vertex0.VertexIndex), tid);
            }

            Edge[] edges = edgeTrisMap.Select(edgeTris => new Edge(edgeTris.Key.Vertex0Index, edgeTris.Key.Vertex1Index, edgeTris.Value.ToArray())).ToArray();

            return new Mesh(vertices, normals, texCoords, triangles, edges);

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
            public readonly int Vertex0Index;
            public readonly int Vertex1Index;

            public EdgeKey(int vertex0Index, int vertex1Index)
            {
                bool isAscendingOrder = vertex0Index < vertex1Index;

                Vertex0Index = isAscendingOrder ? vertex0Index : vertex1Index;
                Vertex1Index = isAscendingOrder ? vertex1Index : vertex0Index;
            }
        }

        private class BareEdgeComparer : IEqualityComparer<EdgeKey>
        {
            public bool Equals(EdgeKey a, EdgeKey b) => a.Vertex0Index == b.Vertex0Index && a.Vertex1Index == b.Vertex1Index;
            public int GetHashCode([DisallowNull] EdgeKey edge) => HashCode.Combine(edge.Vertex0Index, edge.Vertex1Index);
        }
    }
}
