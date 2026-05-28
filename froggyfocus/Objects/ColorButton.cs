using Godot;

public enum ColorButtonType
{
    Red, Yellow, Blue
}

public partial class ColorButton : Area3D
{
    [Export]
    public ColorButtonType Type;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer3D SfxPress;

    private bool is_down;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += PlayerEntered;
        ColorButtonController.Instance.OnTypeChanged += TypeChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ColorButtonController.Instance.OnTypeChanged -= TypeChanged;
    }

    private void PlayerEntered(GodotObject go)
    {
        if (is_down) return;

        SfxPress.Play();
        SetDown(true);
        ColorButtonController.Instance.ChangeType(Type);
    }

    private void TypeChanged(ColorButtonType type)
    {
        SetDown(type == Type);
    }

    private void SetDown(bool is_down)
    {
        if (this.is_down == is_down) return;
        this.is_down = is_down;
        AnimationPlayer.Play(is_down ? "down" : "up");
    }
}
