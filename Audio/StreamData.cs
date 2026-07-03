#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

using Godot;

public class StreamData
{
    public AudioModule.FINISH_MODE FinishMode = AudioModule.FINISH_MODE.DESTROY_IF_QUEUE_EMPTY;
    public AudioStream Stream = null;

    public AudioModule ModuleSource = null;
    public AudioModule Module = null;

    public float VolumeDb = 0f;
    public float Pitch = 1f;

    public float MaxDistance = 0f;
    public float UnitSize = 10f;
}
#endif