#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio;

using System.Collections.Generic;
using Godot;
using Core;

public partial class Audio2D : AudioStreamPlayer2D, IAudio
{
    public System.Action<StreamData, StreamData> HasFinished { get; set; }
    private readonly Queue<AudioModule> _queue = [];

    public AudioModule.FINISH_MODE FinishMode { get; set; }
    public bool KeepAlive { get; set; }

    public AudioModule Module { get => Data.NotNull() ? Data.Module : null; }
    public AudioModule SourceModule { get; set; }
    public StreamData Data { get; private set; }

    public float Length { get => Stream.NotNull() ? (float)Stream.GetLength() : 0f; }
    public float Volume01 { get => VolumeLinear; set => VolumeLinear = value; }
    public float Pitch { get => PitchScale; set => PitchScale = value; }
    public new string Bus { get => base.Bus; set => base.Bus = value; }
    public Queue<AudioModule> Queue { get => _queue; }

    private float _time;
    public float Time
    {
        get => IsPlaying() ? GetPlaybackPosition() : _time;
        set
        {
            _time = value;
            if (IsPlaying()) Seek(value);
        }
    }

    public override void _EnterTree()
    {
        Finished += _Finished;
        base._EnterTree();
    }

    public override void _ExitTree()
    {
        Finished -= _Finished;
        base._ExitTree();
    }

    private void _Finished()
    {
        Callable.From(Next).CallDeferred();
    }

    public new void Play(float from = -1f) => Play(from, null);

    public void Play(float from, AudioModule overwrite)
    {
        var hasOverwrite = overwrite.NotNull();
        var module = hasOverwrite ? overwrite : SourceModule;

        // Resuming the same module tree (no overwrite) keeps its resolved path
        // (current loop/chain position). Anything else starts fresh at index 0.
        var path = !hasOverwrite && Data.NotNull() && Data.OriginModule == SourceModule
            ? Data.Path
            : null;

        PlayPath(module, path, from >= 0f ? from : 0f);
    }

    public void PlayPath(AudioModule module, List<ushort> path, float from = 0f)
    {
        Data = AudioModule.GetStreamData(module, this, path);
        if (Data.IsNull())
        {
            Stop();
            return;
        }

        FinishMode = Data.FinishMode;
        Stream = Data.Stream;

        VolumeDb = Data.VolumeDb;
        Pitch = Data.Pitch;

        MaxDistance = Data.MaxDistance;

        if (Stream.IsNull())
        {
            Stop();
            return;
        }

        _time = Mathf.Clamp(from, 0f, (float)Stream.GetLength());
        base.Play(_time);
    }

    public void ResetData() => Data = null;

    public new void Stop()
    {
        _time = GetPlaybackPosition();
        base.Stop();
    }

    public void Next() => new AudioMarshal(this).Next();

    public void Restart() => Play();
    public void Resume() => Play(Time);

    public void Replace(AudioModule module) => new AudioMarshal(this).Replace(module);

    public void Enqueue(params AudioModule[] modules) => new AudioMarshal(this).Enqueue(modules);

    public void ClearQueue() => new AudioMarshal(this).ClearQueue();

    public void Clear() => new AudioMarshal(this).Clear();

    public void Emit(object obj) => new AudioMarshal(this).Emit(obj);

    public void Insert(AudioModule module, int index) => new AudioMarshal(this).Insert(module, index);
}
#endif