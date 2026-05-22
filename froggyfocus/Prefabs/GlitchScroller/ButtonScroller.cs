using Godot;
using System;

public partial class ButtonScroller : Node2D
{
    [Export]
    public ColorType Color;

    [Export]
    public Sprite2D Sprite;

    [Export]
    public TriggerArea2D PlayerArea;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public AudioStreamPlayer SfxPress;

    public static Action<ColorType> OnButtonPressed;
    public enum ColorType { None, Red, Yellow, Blue }

    private bool pressed = false;

    public override void _Ready()
    {
        base._Ready();
        Sprite.Modulate = GetColor(Color);
        PlayerArea.OnNodeEntered += Player_Entered;
        OnButtonPressed += Button_Pressed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        OnButtonPressed -= Button_Pressed;
    }

    private void Player_Entered(Node2D node)
    {
        if (pressed) return;

        SfxPress.Play();
        OnButtonPressed?.Invoke(Color);
    }

    private void Button_Pressed(ColorType type)
    {
        var should_press = Color == type;
        if (should_press == pressed) return;

        pressed = should_press;
        AnimationPlayer.Play(pressed ? "press" : "release");
    }

    public static Color GetColor(ColorType type) => type switch
    {
        ColorType.None => Colors.Green,
        ColorType.Red => Colors.Red,
        ColorType.Yellow => Colors.Yellow,
        ColorType.Blue => Colors.Blue,
    };
}
