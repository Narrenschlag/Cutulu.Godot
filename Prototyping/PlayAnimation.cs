namespace Cutulu.Prototyping;

using Godot;

[GlobalClass]
public partial class PlayAnimation : AnimationPlayer
{
    [Export] private string AnimationName;

    public override void _Ready()
    {
        base._Ready();

        if (HasAnimation(AnimationName))
            Play(AnimationName);
    }
}