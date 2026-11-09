#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

using Godot;
using Core;

[GlobalClass]
public partial class AudioFile : AudioModule
{
    [Export] public AudioStream File;

    /// <summary> Number of times to loop the file. Set to 0 to loop indefinitely. </summary>
    [Export] public int LoopCount = 1;

    [Export] public Vector2 Volume = Vector2.Zero;
    [Export] public Vector2 Pitch = Vector2.One;

    public override AudioStream GetStream(ushort idx, IAudio audio) => File;

    public override bool HasIdx(ushort idx) => LoopCount < 1 || idx < LoopCount;

    public override void _Apply(ushort idx, IAudio audio, StreamData data)
    {
        base._Apply(idx, audio, data);

        data.VolumeDb *= Random.Range(Mathf.Min(Volume.X, Volume.Y), Mathf.Max(Volume.X, Volume.Y));
        data.Pitch *= Mathf.Max(Random.Range(Mathf.Min(Pitch.X, Pitch.Y), Mathf.Max(Pitch.X, Pitch.Y)), 0.01f);
    }
}
#endif