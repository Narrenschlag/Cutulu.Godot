namespace Cutulu.Core;

public abstract class AssetLoader
{
    public abstract bool TryLoadValue(File file, out object output);
}