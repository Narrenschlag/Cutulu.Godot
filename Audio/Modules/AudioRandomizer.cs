#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio
{
    using Godot;
    using Core;

    [GlobalClass]
    public partial class AudioRandomizer : AudioModule
    {
        [Export] public AudioModule[] Modules;

        [Export] public Vector2 Volume = Vector2.Zero;
        [Export] public Vector2 Pitch = Vector2.One;

        public override AudioInstance GetInstance()
        {
            var instance = Modules.RandomElement().GetInstance();

            if (instance.NotNull())
            {
                instance.Volume += GetVolume();
                instance.Pitch += GetPitch() - 1.0f;
            }

            return instance;
        }

        public virtual float GetVolume() => Random.Range(Mathf.Min(Volume.X, Volume.Y), Mathf.Max(Volume.X, Volume.Y));
        public virtual float GetPitch() => Mathf.Max(Random.Range(Mathf.Min(Pitch.X, Pitch.Y), Mathf.Max(Pitch.X, Pitch.Y)), 0.01f);
    }
}
#endif