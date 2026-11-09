namespace Cutulu.Core;

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using System;
using Godot;

using Lock = System.Threading.Lock;

/// <summary>
/// This asset library solution uses the a file's file name without the file type ending as the asset id.
/// It has to be initialized with the asset directories to crawl for files.
/// </summary>
public static class Assets
{
    public static readonly Dictionary<string, AssetLoader> ExtensionLoaders = [];
    public static int FileLoadsPerFrame = 10;

    private static readonly Dictionary<Type, List<uint>> ByType = [];
    private static readonly Dictionary<string, uint> ByAssetId = [];
    private static readonly List<object> Cache = new(256);
    private static readonly Lock _lock = new();

    public static int Count => Cache.Count;
    private static uint Loaded = 0;

    public static bool IsLoading { get; private set; } = false;

    public static void ClearAssets()
    {
        lock (_lock)
        {
            ByAssetId.Clear();
            ByType.Clear();
            Cache.Clear();
        }
    }

    #region Sync

    public static uint LoadAssets(params string[] assetDirectories)
    {
        IsLoading = true;
        uint sum = 0;
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


    public static uint LoadAssetsFromDirectory(string directory)
    {
        if (directory.IsEmpty()) return 0;

        Directory dir = new(directory, false);

        if (dir.Exists() == false)
        {
            Debug.LogError($"Directory '{directory}' does not exist! You may need to install the asset pack.");
            return 0;
        }

        uint sum = 0;

        File[] files = [.. dir.GetSubFiles()];
        foreach (var file in files)
        {
            if (LoadFile(file.GodotPath, out _, out string assetId, out var type))  // Use GodotPath for ResourceLoader compatibility
            {
                Debug.LogR($"[b]+[/b]   [color=seagreen]{file.SystemPath}[/color] [{assetId}, {type.FullName}]");
                sum++;
            }

            else Debug.LogError($"Unable to load asset: {file.SystemPath} [{assetId}, {type?.FullName ?? "NO_TYPE"}]");
        }

        Directory[] directories = [.. dir.GetSubDirectories()];

        foreach (var subDir in directories) sum += LoadAssetsFromDirectory(subDir.GodotPath);

        return sum;
    }

    #endregion

    #region Async

    public static async Task<uint> LoadAssetsAsync(Action<float, UNumber32> progressFeedback, params string[] assetDirectories)
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


    public static async Task<UNumber32> LoadAssetsFromDirectoryAsync(string directory)
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
            if (LoadFile(file.GodotPath, out _, out string assetId, out Type type))  // Use GodotPath for ResourceLoader compatibility
            {
                sum++;

                Debug.LogR($"[b]+[/b]   [color=seagreen]{file.SystemPath}[/color] [{assetId}, {type.FullName}]");

                if (++Loaded % (uint)FileLoadsPerFrame == 0)
                    await Cutulu.Async.TaskUtils.NextFrame();
            }
            else
            {
                Debug.LogError($"Unable to load asset: {file.SystemPath} [{assetId}, {type?.FullName ?? "NO_TYPE"}]");
            }
        }

        Directory[] directories = [.. dir.GetSubDirectories()];

        foreach (var subDir in directories) sum += await LoadAssetsFromDirectoryAsync(subDir.GodotPath);

        return sum;
    }

    #endregion

    public static bool TryGetAssets<T>(out List<T> value)
    {
        lock (_lock)
            if (ByType.TryGetValue(typeof(T), out var list) && list.NotEmpty())
            {
                value = new List<T>(list.Count);

                for (ushort i = 0; i < list.Count; i++)
                    if (_InternalTryGet(list[i], out T t))
                        value.Add(t);

                return value.Count > 0;
            }

        value = null;
        return false;
    }

    public static bool TryGetAsset<T>(string assetId, out T value)
    {
        lock (_lock)
            if (ByAssetId.TryGetValue(assetId, out var idx))
                return _InternalTryGet(idx, out value);

        value = default;
        return false;
    }

    public static bool TryGetAsset<T>(uint idx, out T value)
    {
        lock (_lock) return _InternalTryGet(idx, out value);
    }

    private static bool _InternalTryGet<T>(uint idx, out T value)
    {
        if (idx < Cache.Count)
        {
            object val = Cache[(int)idx];

            switch (val)
            {
                case TempJsonObject json:
                    T json2 = JsonSerializer.Deserialize<T>(json.PlainText);
                    if (json2.NotNull())
                    {
                        OverwriteAsset(idx, json2, out _);
                        value = json2;
                        return true;
                    }
                    break;

                default:
                    if (val is T t)
                    {
                        value = t;
                        return value.NotNull();
                    }
                    break;
            }
        }

        value = default;
        return false;
    }

    public static bool LoadFile(string filePath, out uint idx, out string assetId, out Type type)
    {
        type = typeof(ArgumentException);
        File file = new(filePath);
        idx = default;

        assetId = Path.GetFileNameWithoutExtension(filePath);
        if (file.Exists() == false) return false;

        string ext = Path.GetExtension(filePath);

        if (ExtensionLoaders.TryGetValue(ext, out var loader) == false || (loader?.TryLoadValue(file, out object value) ?? false) == false || value.IsNull())
            switch (ext)
            {
                // Json
                case ".json":
                    var json = file.ReadString();
                    if (json.IsEmpty()) return false;

                    type = typeof(TempJsonObject);
                    value = new TempJsonObject(json);
                    break;

                // Godot Resource
                default:
                    Resource res = ResourceLoader.Load(file.GodotPath, cacheMode: ResourceLoader.CacheMode.Reuse);
                    if (res.IsNull()) return false;

                    type = res.GetType();
                    value = res;
                    break;
            }

        if (value is not null)
        {
            RegisterAsset(assetId, value, out idx);
            return true;
        }

        else return false;
    }

    /// <summary>
    /// Overwrites the asset value at given asset idx.
    /// </summary>
    public static bool OverwriteAsset(uint idx, object newValue, out object oldValue)
    {
        int i = (int)idx;

        lock (_lock)
        {
            if (Cache.Count < idx)
            {
                oldValue = Cache[i];
                Cache[i] = newValue;
                return true;
            }
        }

        oldValue = null;
        return false;
    }

    private static bool RegisterAsset(string assetId, object value, out uint idx)
    {
        idx = default;

        if (value is null) return false;

        Type type = value.GetType();

        lock (_lock)
        {
            idx = (uint)Cache.Count;

            if (ByType.TryGetValue(type, out var cache) == false)
                ByType[type] = cache = [];

            ByAssetId[assetId] = idx;
            Cache.Add(value);
            cache.Add(idx);
        }

        return true;
    }

    private record struct TempJsonObject(string PlainText);
}