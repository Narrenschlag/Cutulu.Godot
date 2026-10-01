#if GODOT4_0_OR_GREATER
namespace Cutulu.Mesh;

using System.Collections.Generic;
using Cutulu.Core;
using Godot;

/// <summary>
/// Ultra-fast mesh builder with additional UV support and with optional normal smoothing. Written by Maximilian Schecklmann 01.10.2026.
/// </summary>
public class FastMeshBuilderUV(int estimatedTriangles = 16384, bool smoothNormals = true) : FastMeshBuilder(estimatedTriangles, smoothNormals)
{
    private SwapbackArray<Vector2> UVs = new(estimatedTriangles * 3);

    public override void Clear()
    {
        base.Clear();
        UVs.Clear();
    }

    /// <summary>Flat (non-welded) triangle with UVs. Ignores SmoothNormals, because welding would fight the UVs.</summary>
    public void AddTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 normal, Vector2 uv0, Vector2 uv1, Vector2 uv2)
    {
        int baseIndex = Vertices.Count;

        Vertices.Add(v0); Vertices.Add(v1); Vertices.Add(v2);
        Normals.Add(normal); Normals.Add(normal); Normals.Add(normal);
        UVs.Add(uv0); UVs.Add(uv1); UVs.Add(uv2);

        Indices.Add(baseIndex);
        Indices.Add(baseIndex + 1);
        Indices.Add(baseIndex + 2);
    }

    /// <summary>Adds everything built so far as a new surface on an existing mesh. Returns the surface index or -1.</summary>
    public int AddSurfaceTo(ArrayMesh mesh)
    {
        if (Vertices.Count < 3) return -1;

        var arrays = new Godot.Collections.Array();
        arrays.Resize((int)Mesh.ArrayType.Max);

        arrays[(int)Mesh.ArrayType.Vertex] = Vertices.AsSpan().ToArray();
        arrays[(int)Mesh.ArrayType.Normal] = Normals.AsSpan().ToArray();
        arrays[(int)Mesh.ArrayType.Index] = Indices.AsSpan().ToArray();
        if (Colors.Count == Vertices.Count) arrays[(int)Mesh.ArrayType.Color] = Colors.AsSpan().ToArray();
        if (UVs.Count == Vertices.Count) arrays[(int)Mesh.ArrayType.TexUV] = UVs.AsSpan().ToArray();

        mesh.AddSurfaceFromArrays(Mesh.PrimitiveType.Triangles, arrays);
        return mesh.GetSurfaceCount() - 1;
    }
}
#endif