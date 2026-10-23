namespace Cutulu.Core;

using Godot;

[GlobalClass]
public abstract partial class AssetResource : Resource, IAsset
{
    [Export] public string AssetId { get; private set; }
    [Export] public bool Disabled { get; private set; }

    public UNumber32 RemoteUId { get; set; }
}

public interface IAsset
{
    public string AssetId { get; }
    public bool Disabled { get; }

    public UNumber32 RemoteUId { get; set; }
}