using System.Collections.Generic;
using System.Numerics;

namespace CpuRenderer3D.Renderers
{
    public class ContourEdgeRenderer<TFragmentData> : IRenderer where TFragmentData : struct
    {
        private record struct FragVertex(int TriangleId, FragmentInput<TFragmentData> FragInput);
        private record struct TriangleVertexKey(int TriangleId, int VertexId);

        private readonly Mesh _mesh;
        private readonly IShaderProgram<TFragmentData> _shaderProgram;

        private readonly Dictionary<TriangleVertexKey, FragmentInput<TFragmentData>> _triangleVerticesCache;
        private readonly Vector3[] _triangleNormals;

        public ContourEdgeRenderer(Mesh mesh, IShaderProgram<TFragmentData> shaderProgram)
        {
            _mesh = mesh;
            _shaderProgram = shaderProgram;
            _triangleVerticesCache = new Dictionary<TriangleVertexKey, FragmentInput<TFragmentData>>(_mesh.GetVertices().Length * 3);
            _triangleNormals = new Vector3[_mesh.GetTriangles().Length];
        }

        public void Render(RenderingContext renderingContext)
        {
            for (int tid = 0; tid < _mesh.GetTriangles().Length; ++tid)
            {
                Triangle triangle = _mesh.GetTriangles()[tid];

                VertexInput vertInput0 = new VertexInput(_mesh.GetVertex(triangle.Vertex0.VertexIndex), _mesh.GetNormal(triangle.Vertex0.NormalIndex), new Vector4(0.8f, 0.0f, 0.0f, 1f), _mesh.GetTexCoord(triangle.Vertex0.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput1 = new VertexInput(_mesh.GetVertex(triangle.Vertex1.VertexIndex), _mesh.GetNormal(triangle.Vertex1.NormalIndex), new Vector4(0.0f, 0.8f, 0.0f, 1f), _mesh.GetTexCoord(triangle.Vertex1.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());
                VertexInput vertInput2 = new VertexInput(_mesh.GetVertex(triangle.Vertex2.VertexIndex), _mesh.GetNormal(triangle.Vertex2.NormalIndex), new Vector4(0.0f, 0.0f, 0.8f, 1f), _mesh.GetTexCoord(triangle.Vertex2.TexCoordIndex), new Vector2(), new Vector2(), new Vector2());

                FragmentInput<TFragmentData> fragInput0 = _shaderProgram.ComputeVertex(vertInput0, renderingContext);
                FragmentInput<TFragmentData> fragInput1 = _shaderProgram.ComputeVertex(vertInput1, renderingContext);
                FragmentInput<TFragmentData> fragInput2 = _shaderProgram.ComputeVertex(vertInput2, renderingContext);

                _triangleVerticesCache[new TriangleVertexKey(tid, triangle.Vertex0.VertexIndex)] = fragInput0;
                _triangleVerticesCache[new TriangleVertexKey(tid, triangle.Vertex1.VertexIndex)] = fragInput1;
                _triangleVerticesCache[new TriangleVertexKey(tid, triangle.Vertex2.VertexIndex)] = fragInput2;

                _triangleNormals[tid] = Vector3.Cross(fragInput0.Position - fragInput1.Position,
                                                      fragInput0.Position - fragInput2.Position);
            }

            for (int eid = 0; eid < _mesh.GetEdges().Length; eid++)
            {
                Edge edge = _mesh.GetEdges()[eid];

                if (edge.Tris.Length == 2)
                {
                    bool triangle0IsCameraFaced = Vector3.Dot(_triangleNormals[edge.Tris[0]], Vector3.UnitZ) > 0;
                    bool triangle1IsCameraFaced = Vector3.Dot(_triangleNormals[edge.Tris[1]], Vector3.UnitZ) > 0;

                    if (!triangle0IsCameraFaced && !triangle1IsCameraFaced) continue;
                    if (triangle0IsCameraFaced == triangle1IsCameraFaced) continue;

                    FragmentInput<TFragmentData> frag0vertInput0 = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[0], edge.Vertex0Index)];
                    FragmentInput<TFragmentData> frag0vertInput1 = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[0], edge.Vertex1Index)];

                    FragmentInput<TFragmentData> frag1vertInput0 = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[1], edge.Vertex0Index)];
                    FragmentInput<TFragmentData> frag1vertInput1 = _triangleVerticesCache[new TriangleVertexKey(edge.Tris[1], edge.Vertex1Index)];

                    if (frag0vertInput0 == frag1vertInput0 && frag0vertInput1 == frag1vertInput1)
                    {
                        CheckAndDrawLine(renderingContext, frag0vertInput0, frag0vertInput1);
                    }
                    else
                    {
                        CheckAndDrawLine(renderingContext, frag0vertInput0, frag0vertInput1);
                        CheckAndDrawLine(renderingContext, frag1vertInput0, frag1vertInput1);
                    }
                }
                else
                {
                    foreach (int triangleId in edge.Tris)
                    {
                        FragmentInput<TFragmentData> fragInput0 = _triangleVerticesCache[new TriangleVertexKey(triangleId, edge.Vertex0Index)];
                        FragmentInput<TFragmentData> fragInput1 = _triangleVerticesCache[new TriangleVertexKey(triangleId, edge.Vertex1Index)];

                        CheckAndDrawLine(renderingContext, fragInput0, fragInput1);
                    }
                }
            }
        }

        private void CheckAndDrawLine(RenderingContext renderingContext, FragmentInput<TFragmentData> fragInput0, FragmentInput<TFragmentData> fragInput1)
        {
            if (-1f < fragInput0.Position.X && fragInput0.Position.X < 1f
             && -1f < fragInput0.Position.Y && fragInput0.Position.Y < 1f
             && -1f < fragInput0.Position.Z && fragInput0.Position.Z < 1f
             && -1f < fragInput1.Position.X && fragInput1.Position.X < 1f
             && -1f < fragInput1.Position.Y && fragInput1.Position.Y < 1f
             && -1f < fragInput1.Position.Z && fragInput1.Position.Z < 1f)
            {
                fragInput0.Position = Vector3.Transform(fragInput0.Position, renderingContext.ProjectionClip) - Vector3.UnitZ * 0.0001f;
                fragInput1.Position = Vector3.Transform(fragInput1.Position, renderingContext.ProjectionClip) - Vector3.UnitZ * 0.0001f;

                Drawer.DrawLine(renderingContext, fragInput0, fragInput1, _shaderProgram);
            }
        }
    }
}