#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio
{
    using System.Collections.Generic;
    using Godot;
    using Core;

    public partial class Audio1D : AudioStreamPlayer, IAudio
    {
        public System.Action<StreamData, StreamData> HasFinished { get; set; }
        private readonly Queue<AudioModule> _queue = [];

        public AudioModule.FINISH_MODE FinishMode { get; set; }
        public bool KeepAlive { get; set; }

        public StreamData Data { get; private set; }
        public AudioModule Module { get; set; }
        public ushort Idx { get; set; }

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

        public new void Play(float from = -1f)
        {
            Data = AudioModule.GetStreamData(Module, Idx, this);
            if (Data.IsNull())
            {
                Stop();
                return;
            }

            FinishMode = Data.FinishMode;
            Stream = Data.Stream;

            VolumeDb = Data.VolumeDb;
            Pitch = Data.Pitch;

            if (Stream.IsNull())
            {
                Stop();
                return;
            }

            _time = from >= 0f ? Mathf.Clamp(from, 0f, (float)Stream.GetLength()) : 0f;
            base.Play(_time);
        }

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
}
#endif