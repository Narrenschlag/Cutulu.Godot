#if GODOT4_0_OR_GREATER
namespace Cutulu.Audio
{
    using Godot;
    using Core;

    public static class Audiof
    {
        public const string DEFAULT_BUS = "Master";

        /// <summary>
        /// Plays audio module at given 3D global position.
        /// </summary>
        public static Audio3D Play(AudioModule module, Node parent, Vector3 global, string bus = DEFAULT_BUS)
        {
            var audio = Create<Audio3D>(parent, bus);

            audio.GlobalPosition = global;
            audio.Enqueue(module);

            return audio;
        }

        /// <summary>
        /// Plays audio module at given 2D global position.
        /// </summary>
        public static Audio2D Play(AudioModule module, Node parent, Vector2 global, string bus = DEFAULT_BUS)
        {
            var audio = Create<Audio2D>(parent, bus);

            audio.GlobalPosition = global;
            audio.Enqueue(module);

            return audio;
        }

        /// <summary>
        /// Plays audio module globally. 
        /// </summary>
        public static Audio1D Play(AudioModule module, Node parent, string bus = DEFAULT_BUS)
        {
            var audio = Create<Audio1D>(parent, bus);

            audio.Enqueue(module);

            return audio;
        }

        private static AUDIO Create<AUDIO>(Node parent, string bus) where AUDIO : Node, IAudio, new()
        {
            if (parent.IsNull()) throw new($"Cannot create audio instance. Parent cannot be null.");
            if (bus.IsEmpty()) throw new($"Cannot create audio instance. Bus cannot be empty.");

            var audio = new AUDIO();
            parent.AddChild(audio);

            audio.Bus = bus;

            return audio;
        }
    }
}
#endif