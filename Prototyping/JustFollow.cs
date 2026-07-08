namespace Cutulu.Prototyping;

using Godot;
using Core;

public partial class JustFollow : Node3D
{
    [Export] public Node3D Target;
    [Export] public float Speed = 4.0f;
    [Export] public bool FollowOnPhysics;

    [Export] public float RotationSpeed = 0f;
    [Export] public Vector3 RotationOffset = default;

    public override void _Process(double delta)
    {
        if (Target.NotNull() && FollowOnPhysics == false)
            _Process((float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target.NotNull() && FollowOnPhysics)
            _Process((float)delta);
    }

    private void _Process(float delta)
    {
        if (Speed > 0f)
            GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition, Speed * delta);

        if (RotationSpeed > 0f)
        {
            var angle = Target.Forward().DirectionToRadians();
            var offset = RotationOffset.ToRadians();

            GlobalRotation = Vector3.Up * Mathf.LerpAngle(GlobalRotation.Y - offset.Y, angle, delta * RotationSpeed) + offset;
        }
    }
}