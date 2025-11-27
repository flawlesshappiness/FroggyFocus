using Godot;

public partial class FloatBounce : Node3D
{
    [Export]
    public Area3D Area;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        base._Ready();
        Area.BodyEntered += BodyEntered;
    }

    private void BodyEntered(GodotObject go)
    {
        AnimationPlayer.Play("bounce");
    }
}
