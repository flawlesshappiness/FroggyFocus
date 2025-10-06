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
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(GodotObject body)
    {
        GameScene.Instance.SetFocusEventId(Id);
    }

    private void OnBodyExited(GodotObject bodt)
    {
        GameScene.Instance.ClearFocusEventId();
    }
}
