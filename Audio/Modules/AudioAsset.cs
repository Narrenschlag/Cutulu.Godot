#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

using Godot;
using Core;

[GlobalClass]
public partial class AudioAsset : AudioModule
{
    [Export] public string StreamAsset;
    [Export] public Vector2 Volume = Vector2.Zero;
    [Export] public Vector2 Pitch = Vector2.One;

    public override AudioStream GetStream(ushort idx, IAudio audio) => Assets.TryGetAsset(StreamAsset, out AudioStream stream) ? stream : null;

    public override void _Apply(ushort idx, IAudio audio, StreamData data)
    {
        base._Apply(idx, audio, data);

        data.VolumeDb *= Random.Range(Mathf.Min(Volume.X, Volume.Y), Mathf.Max(Volume.X, Volume.Y));
        data.Pitch *= Mathf.Max(Random.Range(Mathf.Min(Pitch.X, Pitch.Y), Mathf.Max(Pitch.X, Pitch.Y)), 0.01f);
    }
}
#endif