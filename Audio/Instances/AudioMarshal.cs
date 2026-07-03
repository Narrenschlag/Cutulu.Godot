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

            else if (Audio.Module.IsNull() && Audio.Queue.Count < 1)
            {
                for (ushort i = 1; i < modules.Length; i++)
                    Audio.Queue.Enqueue(modules[i]);

                Replace(modules[0]);
            }

            else
            {
                for (ushort i = 0; i < modules.Length; i++)
                    Audio.Queue.Enqueue(modules[i]);

                if (Audio.Module.IsNull()) Next();
            }
        }

        public void Replace(AudioModule module)
        {
            Audio.Module = module;
            Audio.Idx = 0;

            if (module.NotNull()) Audio.Play();
            else Audio.Stop();
        }

        public void Next()
        {
            var old = Audio.Data;

            if (Audio.KeepAlive)
            {
                if (Audio.Queue.IsEmpty())
                {
                    if (Audio.Module.HasIdx(++Audio.Idx))
                    {
                        Audio.Play();
                        Audio.HasFinished?.Invoke(old, old);
                    }

                    else Audio.HasFinished?.Invoke(old, null);

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

            if (Audio.Module.NotNull())
            {
                if (Audio.Module.HasIdx(++Audio.Idx))
                {
                    Audio.Play();
                    return;
                }

                Audio.Module._Finished(this);
            }

            Replace(Audio.Queue.Count > 0 ? Audio.Queue.Dequeue() : null);

            Audio.HasFinished?.Invoke(old, Audio.Data);
        }

        public void ClearQueue()
        {
            Audio.Queue.Clear();
        }

        public void Clear()
        {
            ClearQueue();
            Audio.Module = null;
            Audio.Stop();
        }

        public void Emit(object obj)
        {
            if (Audio.Module.NotNull())
                Audio.Module._Emited(this, obj);
        }

        public void Insert(AudioModule module, int index)
        {
            if (Audio.Module.IsNull() && Audio.Queue.Count < 1)
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