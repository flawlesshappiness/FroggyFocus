using Godot;

public partial class ScrollerWaterArea : Area2D
{
    public override void _Ready()
    {
        base._Ready();
        BodyEntered += Player_Entered;
    }

    private void Player_Entered(GodotObject go)
    {
        var node = go as Node2D;
        var player = node as SideScrollerController;
        player.Respawn();
    }
}
