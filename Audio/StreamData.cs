#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

using System.Collections.Generic;
using Godot;

public class StreamData
{
    public AudioModule.FINISH_MODE FinishMode = AudioModule.FINISH_MODE.DESTROY_IF_QUEUE_EMPTY;
    public AudioStream Stream = null;

    public AudioModule OriginModule = null;
    public AudioModule Module = null;

    public float VolumeDb = 0f;
    public float Pitch = 1f;

    public float MaxDistance = 0f;
    public float UnitSize = 10f;

    /// <summary>
    /// The local index used at each depth of the module tree that produced this
    /// StreamData. Path[0] belongs to OriginModule, Path[^1] to the resolved leaf (Module).
    /// </summary>
    public List<ushort> Path = [];

    /// <summary>
    /// The module resolved at each depth, parallel to Path.
    /// </summary>
    public List<AudioModule> ModulePath = [];
}
#endif