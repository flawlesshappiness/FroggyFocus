using Godot;

public partial class AnimatedCoin : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        base._Ready();
        AnimationPlayer.Play("spinning");
    }
}
