#if GODOT4_0_OR_GREATER
namespace Cutulu.Core
{
    using Godot;

    public partial class AutoRotate : Node3D
    {
        [Export] public Vector3 Speed;

        public override void _Process(double delta)
        {
            Rotation += new Vector3(Mathf.DegToRad(Speed.X), Mathf.DegToRad(Speed.Y), Mathf.DegToRad(Speed.Z)) * (float)delta;
        }
    }
}
#endif