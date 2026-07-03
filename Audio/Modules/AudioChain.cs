#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

using Godot;
using Core;

[GlobalClass]
public partial class AudioChain : AudioModule
{
    [Export] public AudioModule[] Modules;
    [Export] public bool Loop = false;

    [Export] public Vector2 Volume = Vector2.Zero;
    [Export] public Vector2 Pitch = Vector2.One;

    public override bool TryGetSubModule(ushort idx, IAudio audio, out AudioModule subModule)
    {
        if (HasIdx(idx) == false) subModule = null;
        else subModule = Modules[audio.Idx = (ushort)((int)idx).AbsMod(Modules.Length)];

        return subModule.NotNull();
    }

    public override bool HasIdx(ushort idx) => Loop ? Modules.NotEmpty() : Modules.Size() > idx;

    public override void _Apply(ushort idx, IAudio audio, StreamData data)
    {
        base._Apply(idx, audio, data);

        data.VolumeDb *= Random.Range(Mathf.Min(Volume.X, Volume.Y), Mathf.Max(Volume.X, Volume.Y));
        data.Pitch *= Mathf.Max(Random.Range(Mathf.Min(Pitch.X, Pitch.Y), Mathf.Max(Pitch.X, Pitch.Y)), 0.01f);
    }
}
#endif