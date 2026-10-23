namespace Cutulu.Core;

using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using Godot;

public partial class AssetManager
{
    public int FileLoadsPerFrame = 10;

    private readonly Dictionary<Type, List<uint>> ByType = [];
    private readonly List<IAsset> Cache = new(256);
    private readonly Dictionary<string, uint> ByRId = [];

    public int Count => Cache.Count;
    private uint Loaded = 0;

    public bool IsLoading { get; private set; } = false;

    #region Sync

    public uint LoadAssets(params string[] assetDirectories)
    {
        UNumber32 sum = 0, dirIdx = 0;
        IsLoading = true;
        Loaded = 0;

        Debug.LogR($"[color=darkorange]Loading assets.");

        foreach (var directory in assetDirectories)
        {
            Debug.LogR($"[b]?[/b] [color=darkorange]Loading[/color] {directory}");
            sum += LoadAssetsFromDirectory(directory);

            Debug.LogR($"[b]#[/b] [color=darkgreen]Loaded[/color] {directory}");
        }

        Debug.LogR($"[color=seagreen]Loaded [b]{sum} asset(s)[/b].");
        IsLoading = false;
        return sum;
    }


    public UNumber32 LoadAssetsFromDirectory(string directory)
    {
        if (directory.IsEmpty()) return 0;

        Directory dir = new(directory, false);

        if (dir.Exists() == false)
        {
            Debug.LogError($"Directory '{directory}' does not exist! You may need to install the asset pack.");
            return 0;
        }

        UNumber32 sum = 0;

        File[] files = [.. dir.GetSubFiles(".tres", ".res")];
        foreach (var file in files)
        {
            if (LoadAsset<IAsset>(this, file.GodotPath, out Type type))  // Use GodotPath for ResourceLoader compatibility
            {
                sum++;

                Debug.LogR($"[b]+[/b]   [color=seagreen]{file.SystemPath}[/color] [{type.FullName}]");
            }
            else
            {
                Debug.LogError($"Unable to load asset: {file.SystemPath} [{type?.FullName ?? "NO_TYPE"}]");
            }
        }

        Directory[] directories = [.. dir.GetSubDirectories()];

        foreach (var subDir in directories) sum += LoadAssetsFromDirectory(subDir.GodotPath);

        return sum;
    }

    #endregion

    #region Async

    public async Task<uint> LoadAssetsAsync(Action<float, UNumber32> progressFeedback, params string[] assetDirectories)
    {
        UNumber32 sum = 0, dirIdx = 0;
        IsLoading = true;
        Loaded = 0;

        Debug.LogR($"[color=darkorange]Loading assets.");

        foreach (var directory in assetDirectories)
        {
            Debug.LogR($"[b]?[/b] [color=darkorange]Loading[/color] {directory}");
            sum += await LoadAssetsFromDirectoryAsync(directory);

            Debug.LogR($"[b]#[/b] [color=darkgreen]Loaded[/color] {directory}");

            progressFeedback?.Invoke(++dirIdx / (float)assetDirectories.Length, sum);
        }

        Debug.LogR($"[color=seagreen]Loaded [b]{sum} asset(s)[/b].");
        IsLoading = false;
        return sum;
    }


    public async Task<UNumber32> LoadAssetsFromDirectoryAsync(string directory)
    {
        if (directory.IsEmpty()) return 0;

        Directory dir = new(directory, false);

        if (dir.Exists() == false)
        {
            Debug.LogError($"Directory '{directory}' does not exist! You may need to install the asset pack.");
            return 0;
        }

        UNumber32 sum = 0;

        File[] files = [.. dir.GetSubFiles(".tres", ".res")];
        foreach (var file in files)
        {
            if (LoadAsset<IAsset>(this, file.GodotPath, out Type type))  // Use GodotPath for ResourceLoader compatibility
            {
                sum++;

                Debug.LogR($"[b]+[/b]   [color=seagreen]{file.SystemPath}[/color] [{type.FullName}]");

                if (++Loaded % (uint)FileLoadsPerFrame == 0)
                    await Cutulu.Async.TaskUtils.NextFrame();
            }
            else
            {
                Debug.LogError($"Unable to load asset: {file.SystemPath} [{type?.FullName ?? "NO_TYPE"}]");
            }
        }

        Directory[] directories = [.. dir.GetSubDirectories()];

        foreach (var subDir in directories) sum += await LoadAssetsFromDirectoryAsync(subDir.GodotPath);

        return sum;
    }

    #endregion

    public static bool LoadAsset<A>(AssetManager manager, string filePath, out Type type) where A : IAsset
    {
        type = default;
        if (ResourceLoader.Exists(filePath) == false) return false;

        Resource res = ResourceLoader.Load(filePath, cacheMode: ResourceLoader.CacheMode.Reuse);

        if (res.IsNull())
        {
            Debug.LogError($"Failed to load asset '{filePath}'");
            return false;
        }

        type = res.GetType();

        if (res is not A original)
        {
            res.Destroy();
            return false;
        }

        if (original.Disabled || original.AssetId.IsEmpty() || manager.ByRId.ContainsKey(original.AssetId))
        {
            // don't destroy, may be the same shared instance already in use
            //original.Destroy(); 
            return false;
        }

        original.RemoteUId = manager.Cache.Count;
        A file = original;

        if (file.IsNull()) return false;

        manager.Cache.Add(file);
        manager.ByRId[file.AssetId] = file.RemoteUId;

        Type _type = file.GetType();
        if (manager.ByType.TryGetValue(_type, out var list) == false)
            manager.ByType[_type] = list = [];
        list.Add(file.RemoteUId);

        Type baseType = _type.BaseType;
        while (baseType != null && typeof(IAsset).IsAssignableFrom(baseType))
        {
            if (manager.ByType.TryGetValue(baseType, out var baseList) == false)
                manager.ByType[baseType] = baseList = [];

            baseList.Add(file.RemoteUId);
            baseType = baseType.BaseType;
        }

        return true;
    }

    public List<A> GetAllAssets<A>() where A : IAsset
    {
        //Debug.LogR($"[GetAllAssets] instance={GetHashCode()} Cache.Count={Cache.Count} IsLoading={IsLoading}");

        var list = new List<A>(Cache.Count / 4);

        for (int i = 0; i < Cache.Count; i++)
            if (Cache[i] is A r && r.NotNull())
                list.Add(r);

        return list;
    }

    public bool TryGet<A>(string AssetId, out A resource) where A : IAsset
    {
        if (AssetId.NotEmpty() && ByRId.TryGetValue(AssetId, out var remoteUId) && remoteUId < Cache.Count && Cache[(int)remoteUId] is A r && r.NotNull())
        {
            resource = r;
            return true;
        }

        resource = default;
        return false;
    }

    public bool TryGet<A>(uint remoteUId, out A resource) where A : IAsset
    {
        if (remoteUId < Cache.Count && Cache[(int)remoteUId] is A r && r.NotNull())
        {
            resource = r;
            return true;
        }

        resource = default;
        return false;
    }

    public A[] Get<A>() where A : IAsset
    {
        var list = new List<A>();

        foreach (Type type in ByType.Keys)
        {
            // FIX: Was IsInstanceOfType(typeof(RR)) which is always false for Type args.
            // IsAssignableFrom correctly checks if cached type is RR or a subclass of RR.
            if (typeof(A).IsAssignableFrom(type) == false) continue;

            foreach (uint remoteUId in ByType[type])
            {
                if (TryGet<A>(remoteUId, out var resource))
                    list.Add(resource);
            }
        }

        return [.. list];
    }
}