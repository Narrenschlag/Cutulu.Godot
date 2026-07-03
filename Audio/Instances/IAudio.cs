#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

using System.Collections.Generic;
using Godot;

public interface IAudio
{
    public System.Action<StreamData, StreamData> HasFinished { get; set; }

    AudioModule.FINISH_MODE FinishMode { get; set; }
    public bool KeepAlive { get; set; }

    AudioStream Stream { get; set; }
    float VolumeDb { get; set; }
    float Volume01 { get; set; }
    float Pitch { get; set; }
    float Time { get; set; }
    string Bus { get; set; }

    public Queue<AudioModule> Queue { get; }
    AudioModule SourceModule { get; set; }
    AudioModule Module { get; }
    public StreamData Data { get; }
    float Length { get; }

    /// <summary>
    /// Tells you if the player is playing.
    /// </summary>
    public bool IsPlaying();

    /// <summary>
    /// Replaces the current module with the given module. Stops the player if module is null.
    /// </summary>
    public void Replace(AudioModule module);

    /// <summary>
    /// Enqueues modules to be played after the current module. If the current module is null, the first module in the queue will be played.
    /// </summary>
    public void Enqueue(params AudioModule[] modules);

    /// <summary>
    /// Inserts the given module at the given index in the queue. If the current module is null, the first module in the queue will be played.
    /// </summary>
    public void Insert(AudioModule module, int index);

    /// <summary>
    /// Clears the queue, does not affect current module.
    /// </summary>
    public void ClearQueue();

    /// <summary>
    /// Clears queue and current module. Stops the player.
    /// </summary>
    public void Clear();

    /// <summary>
    /// Emits the given object to the current module.
    /// </summary>
    public void Emit(object obj);

    /// <summary>
    /// Play current module from the given time.
    /// </summary>
    public void Play(float from = -1f) => Play(from, null);

    /// <summary>
    /// Play current module from the given time. Overwrites the source module and
    /// always resolves a fresh path (starts the tree over from index 0 at every depth).
    /// </summary>
    public void Play(float from, AudioModule overwrite);

    /// <summary>
    /// Plays the given module tree using an explicit per-depth resolution path.
    /// Used internally to advance loops/chains without losing sibling state.
    /// </summary>
    public void PlayPath(AudioModule module, List<ushort> path, float from = 0f);

    /// <summary>
    /// Clears the current playback data without stopping or changing SourceModule.
    /// </summary>
    public void ResetData();

    /// <summary>
    /// Play next module in the queue. If the queue is empty this stops the player.
    /// </summary>
    public void Next();

    /// <summary>
    /// Stops the player. Does not affect current module.
    /// </summary>
    public void Stop();

    /// <summary>
    /// Restarts the player. Does not affect current module.
    /// </summary>
    public void Restart();

    /// <summary>
    /// Resumes the player. Does not affect current module.
    /// </summary>
    public void Resume();
}
#endif