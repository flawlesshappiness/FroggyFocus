using Godot;

public partial class PlatformScroller : Node2D
{
    [Export]
    public ButtonScroller.ColorType Color;

    [Export]
    public CollisionShape2D Collider;

    [Export]
    public Sprite2D SpriteSolid;

    [Export]
    public Sprite2D SpriteClear;

    public override void _Ready()
    {
        base._Ready();
        SpriteSolid.Modulate = ButtonScroller.GetColor(Color);
        SpriteClear.Modulate = ButtonScroller.GetColor(Color);
        SetSolid(Color == ButtonScroller.ColorType.None);
        ButtonScroller.OnButtonPressed += Button_Pressed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        ButtonScroller.OnButtonPressed -= Button_Pressed;
    }

    private void Button_Pressed(ButtonScroller.ColorType type)
    {
        SetSolid(Color == ButtonScroller.ColorType.None || Color == type);
    }

    public void SetSolid(bool solid)
    {
        SpriteSolid.Visible = solid;
        SpriteClear.Visible = !solid;
        Collider.SetDeferred("disabled", !solid);
    }
}
