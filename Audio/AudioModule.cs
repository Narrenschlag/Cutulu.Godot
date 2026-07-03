#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

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
        data.Stream = GetStream(idx, audio);
        data.FinishMode = FinishMode;
        data.Module = this;
    }

    public static StreamData GetStreamData(AudioModule module, ushort idx, IAudio audio)
    {
        if (module.IsNull()) return null;

        SwapbackArray<AudioModule> history = [];
        StreamData data = new()
        {
            ModuleSource = module,
        };

        while (history.Contains(module) == false)
        {
            if (module.NotNull()) module._Apply(idx, audio, data);
            history.Add(module);

            if (module.TryGetSubModule(idx, audio, out var subModule) && subModule.NotNull()) module = subModule;

            else break;
        }

        return data;
    }

    public enum FINISH_MODE : byte
    {
        NONE,
        DESTROY_IF_QUEUE_EMPTY,
        DESTROY_ALWAYS,
    }
}
#endif