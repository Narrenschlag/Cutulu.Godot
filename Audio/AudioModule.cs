#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

using System.Collections.Generic;
using Godot;
using Core;

[GlobalClass]
public abstract partial class AudioModule : Resource
{
    [Export] public FINISH_MODE FinishMode = FINISH_MODE.DESTROY_IF_QUEUE_EMPTY;

    public virtual AudioStream GetStream(ushort idx, IAudio audio) => null;

    public virtual bool TryGetSubModule(ushort idx, IAudio audio, out AudioModule subModule)
    {
        subModule = null;
        return false;
    }

    public virtual bool HasIdx(ushort idx) => false;

    public virtual void _Emited(AudioMarshal marshal, object obj) { }

    public virtual void _Finished(AudioMarshal marshal) { }

    public virtual void _Apply(ushort idx, IAudio audio, StreamData data)
    {
        var stream = GetStream(idx, audio);

        // Clean up the stream
        if (stream.NotNull())
        {
            switch (stream)
            {
                case AudioStreamOggVorbis ogg when ogg.Loop:
                    ogg = (AudioStreamOggVorbis)ogg.Duplicate();
                    ogg.Loop = false;
                    stream = ogg;
                    break;

                case AudioStreamMP3 mp3 when mp3.Loop:
                    mp3 = (AudioStreamMP3)mp3.Duplicate();
                    mp3.Loop = false;
                    stream = mp3;
                    break;

                case AudioStreamWav wav when wav.LoopMode != AudioStreamWav.LoopModeEnum.Disabled:
                    wav = (AudioStreamWav)wav.Duplicate();
                    wav.LoopMode = AudioStreamWav.LoopModeEnum.Disabled;
                    stream = wav;
                    break;

                default: break;
            }
        }

        data.Stream = stream;

        data.FinishMode = FinishMode;
        data.Module = this;
    }

    /// <summary>
    /// Resolves a full StreamData by walking the module tree. `path` supplies the
    /// local index to use at each depth (root = depth 0). Missing/deeper entries
    /// default to 0. The path actually used at each depth is recorded on the result,
    /// alongside the module resolved at that depth (ModulePath) so Advance() can
    /// later call HasIdx on the correct module per level.
    /// </summary>
    public static StreamData GetStreamData(AudioModule sourceModule, IAudio audio, List<ushort> path = null)
    {
        if (sourceModule.IsNull()) return null;

        StreamData data = new()
        {
            OriginModule = sourceModule,
        };

        var module = sourceModule;
        var depth = 0;

        SwapbackArray<AudioModule> history = [];

        while (history.Contains(module) == false)
        {
            var localIdx = path is not null && depth < path.Count ? path[depth] : (ushort)0;

            data.Path.Add(localIdx);
            data.ModulePath.Add(module);

            module._Apply(localIdx, audio, data);
            history.Add(module);

            if (module.TryGetSubModule(localIdx, audio, out var subModule) && subModule.NotNull())
            {
                module = subModule;
                depth++;
            }

            else break;
        }

        return data;
    }

    /// <summary>
    /// Odometer-style advance: tries to move the deepest level of the given
    /// StreamData's path forward first, carrying up to shallower levels whenever
    /// a level is exhausted (its module's HasIdx returns false for the next index).
    /// Deeper levels are truncated after a carry so they resolve fresh (index 0)
    /// on the next GetStreamData call. Returns null once every level - including
    /// the root - is exhausted.
    /// </summary>
    public static List<ushort> Advance(StreamData data)
    {
        if (data is null || data.Path.Count == 0) return null;

        var path = new List<ushort>(data.Path);

        for (var depth = path.Count - 1; depth >= 0; depth--)
        {
            var candidate = (ushort)(path[depth] + 1);

            if (data.ModulePath[depth].HasIdx(candidate))
            {
                path[depth] = candidate;

                if (depth + 1 < path.Count)
                    path.RemoveRange(depth + 1, path.Count - depth - 1);

                return path;
            }
        }

        return null;
    }

    public enum FINISH_MODE : byte
    {
        NONE,
        DESTROY_IF_QUEUE_EMPTY,
        DESTROY_ALWAYS,
    }
}
#endif