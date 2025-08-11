using Godot;

[GlobalClass]
public partial class PlayerGround : Area3D
{
    [Export]
    public string Id;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += PlayerEntered;
    }

    private void PlayerEntered(GodotObject go)
    {
        if (!IsInstanceValid(go)) return;

        Player.Instance.SetMoveSoundsId(Id);
    }
}
