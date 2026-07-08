namespace Cutulu.Prototyping;

using Godot;
using Core;

public partial class JustFollow : Node3D
{
    [Export] public Node3D Target;
    [Export] public float Speed = 4.0f;
    [Export] public bool FollowOnPhysics;

    public override void _Process(double delta)
    {
        if (Target.IsNull() || FollowOnPhysics) return;

        GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition, Speed * (float)delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Target.IsNull() || FollowOnPhysics == false) return;

        GlobalPosition = GlobalPosition.Lerp(Target.GlobalPosition, Speed * (float)delta);
    }
}