using Godot;

[GlobalClass]
public partial class FocusEventArea : Area3D
{
    [Export]
    public string Id;

    [Export]
    public int MaxRarity = -1;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    private void OnBodyEntered(GodotObject body)
    {
        Player.Instance.MaxRarity = MaxRarity;
        GameScene.Instance.SetFocusEventId(Id);
    }

    private void OnBodyExited(GodotObject bodt)
    {
        Player.Instance.MaxRarity = -1;
        GameScene.Instance.ClearFocusEventId();
    }
}
