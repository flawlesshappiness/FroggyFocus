using Godot;

[GlobalClass]
public partial class FocusEventArea : Area3D
{
    [Export]
    public string Id;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(GodotObject body)
    {
        var node = body as Node3D;
        GameScene.Instance.SetFocusEventId(Id);
    }
}
