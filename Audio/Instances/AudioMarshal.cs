#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio
{
    using System.Collections.Generic;
    using Core;

    public ref struct AudioMarshal(IAudio audio)
    {
        public readonly IAudio Audio = audio;

        public void Enqueue(params AudioModule[] modules)
        {
            if (modules.IsEmpty()) return;

            else if (Audio.SourceModule.IsNull() && Audio.Queue.Count < 1)
            {
                for (ushort i = 1; i < modules.Length; i++)
                    Audio.Queue.Enqueue(modules[i]);

                Replace(modules[0]);
            }

            else
            {
                for (ushort i = 0; i < modules.Length; i++)
                    Audio.Queue.Enqueue(modules[i]);

                if (Audio.SourceModule.IsNull()) Next();
            }
        }

        public void Replace(AudioModule module)
        {
            Audio.SourceModule = module;

            // overwrite = module forces GetStreamData to resolve fresh (path = null),
            // so Replace always restarts from index 0 at every depth - even when
            // re-Replacing with the exact same module reference.
            if (module.NotNull()) Audio.Play(0f, module);
            else
            {
                Audio.ResetData();
                Audio.Stop();
            }
        }

        public void Next()
        {
            var old = Audio.Data;

            if (Audio.KeepAlive)
            {
                if (Audio.Queue.IsEmpty())
                {
                    if (TryAdvance(old)) return;

                    Audio.HasFinished?.Invoke(old, null);
                    return;
                }
            }

            else
            {
                switch (Audio.FinishMode)
                {
                    case AudioModule.FINISH_MODE.NONE:
                        break;

                    case AudioModule.FINISH_MODE.DESTROY_IF_QUEUE_EMPTY:
                        if (Audio.Queue.IsEmpty()) Audio.Destroy();
                        break;

                    case AudioModule.FINISH_MODE.DESTROY_ALWAYS:
                        Audio.Destroy();
                        break;
                }
            }

            if (TryAdvance(old)) return;

            if (Audio.SourceModule.NotNull())
                Audio.SourceModule._Finished(this);

            Replace(Audio.Queue.Count > 0 ? Audio.Queue.Dequeue() : null);

            Audio.HasFinished?.Invoke(old, Audio.Data);
        }

        /// <summary>
        /// Tries to advance the current module tree to its next valid loop/chain
        /// position, odometer-style (deepest level first, carrying up to shallower
        /// levels when a level is exhausted). Returns false if the whole tree - all
        /// the way up to OriginModule - is exhausted.
        /// </summary>
        private readonly bool TryAdvance(StreamData data)
        {
            if (data.IsNull() || data.OriginModule.IsNull()) return false;

            var nextPath = AudioModule.Advance(data);
            if (nextPath is null) return false;

            Audio.PlayPath(data.OriginModule, nextPath, 0f);
            return true;
        }

        public void ClearQueue()
        {
            Audio.Queue.Clear();
        }

        public void Clear()
        {
            ClearQueue();
            Audio.SourceModule = null;
            Audio.ResetData();
            Audio.Stop();
        }

        public void Emit(object obj)
        {
            if (Audio.SourceModule.NotNull())
                Audio.SourceModule._Emited(this, obj);
        }

        public void Insert(AudioModule module, int index)
        {
            if (Audio.SourceModule.IsNull() && Audio.Queue.Count < 1)
            {
                Replace(module);
                return;
            }

            var list = new List<AudioModule>(Audio.Queue);
            if (index < 0) index = 0;
            if (index > list.Count) index = list.Count;

            list.Insert(index, module);

            ClearQueue();
            Enqueue(list.ToArray());
        }
    }
}
#endif