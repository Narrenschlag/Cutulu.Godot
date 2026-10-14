namespace Cutulu.Mesh;

using System.Collections.Generic;
using System;
using Godot;
using Core;

public static class MeshExporter
{
    /// <summary>
    /// Surfaces 0..N-1 map to <paramref name="surfaceColorIds"/>;
    /// any extra trailing surface (the backdrop) gets the seam material.
    /// </summary>
    public static Error ExportToGltf(
        this ArrayMesh mesh,
        string path,
        IReadOnlyList<short> surfaceColorIds,
        Func<short, Material> colorToMaterial,
        Material seamMaterial)
    {
        var count = mesh.GetSurfaceCount();
        var mats = new Material[count];

        for (var i = 0; i < count; i++)
        {
            var isGlass = i < surfaceColorIds.Count;
            var mat = isGlass ? colorToMaterial(surfaceColorIds[i]) : seamMaterial;
            if (mat == null) continue;

            // Duplicate so renaming doesn't touch your shared resources,
            // and so each surface gets its own named slot in Blender.
            var copy = (Material)mat.Duplicate();
            copy.ResourceName = isGlass ? $"Glass_{surfaceColorIds[i]}" : "Seam";
            mats[i] = copy;
        }

        return ExportToGltf((Mesh)mesh, path, mats);
    }

    /// <summary>
    /// One material per surface (index = surface id). Null entries keep
    /// whatever material is already set on the mesh surface.
    /// </summary>
    public static Error ExportToGltf(this Mesh mesh, string path, params Material[] surfaceMaterials)
    {
        var root = new Node3D { Name = "Root" };
        var instance = new MeshInstance3D { Name = "GeneratedMesh", Mesh = mesh };
        root.AddChild(instance);
        instance.Owner = root;

        var surfaces = mesh.GetSurfaceCount();
        for (var i = 0; i < surfaceMaterials.Length && i < surfaces; i++)
        {
            var mat = surfaceMaterials[i];
            if (mat == null) continue;

            if (string.IsNullOrEmpty(mat.ResourceName))
                mat.ResourceName = $"Material_{i}";

            instance.SetSurfaceOverrideMaterial(i, mat);
        }

        var err = Write(root, path);
        root.Destroy(0, true); // temp scene isn't in the tree, so free it manually
        return err;
    }

    public static Error ExportToGltf(this MeshInstance3D source, string path)
    {
        if (source.Mesh == null) return Error.InvalidParameter;

        var root = new Node3D { Name = "Root" };
        var instance = new MeshInstance3D { Name = "GeneratedMesh", Mesh = source.Mesh };
        root.AddChild(instance);
        instance.Owner = root;

        var surfaces = source.Mesh.GetSurfaceCount();
        for (var i = 0; i < surfaces; i++)
        {
            // Override set on the node (glass colors) wins, otherwise the
            // material stored on the mesh itself (your seam material).
            var mat = source.GetSurfaceOverrideMaterial(i) ?? source.Mesh.SurfaceGetMaterial(i);
            if (mat == null) continue;

            var copy = (Material)mat.Duplicate();
            if (string.IsNullOrEmpty(copy.ResourceName))
                copy.ResourceName = $"Material_{i}";

            instance.SetSurfaceOverrideMaterial(i, copy);
        }

        var err = Write(root, path);
        root.Destroy(0, true); // temp scene isn't in the tree
        return err;
    }

    private static Error Write(Node root, string path)
    {
        var doc = new GltfDocument();
        var state = new GltfState();

        var err = doc.AppendFromScene(root, state);
        return err == Error.Ok ? doc.WriteToFilesystem(state, path) : err;
    }
}