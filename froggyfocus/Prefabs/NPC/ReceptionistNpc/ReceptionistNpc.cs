using FlawLizArt.Animation.StateMachine;
using Godot;

public partial class ReceptionistNpc : Node3D
{
    [Export]
    public CuteFrogCharacter Frog;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public Color PantsColor;

    public bool Looking { set => param_looking.Set(value); }

    public BoolParameter param_looking = new BoolParameter("look", false);

    public override void _Ready()
    {
        base._Ready();
        InitializeAnimations();
        InitializeAppearance();
    }

    private void InitializeAnimations()
    {
        var idle = Animation.CreateAnimation("Armature|idle_table", true);
        var look = Animation.CreateAnimation("Armature|look_table", true);

        Animation.Connect(idle, look, param_looking.WhenTrue());
        Animation.Connect(look, idle, param_looking.WhenFalse());

        Animation.Start(idle.Node);
    }

    private void InitializeAppearance()
    {
        Frog.ClearAppearance();
        var base_color = new Color(0.2f, 0.7f, 0.1f);
        Frog.SetBodyBase(base_color);
        Frog.SetBodyEye(ItemType.BodyEye_Derp, Colors.White);
        Frog.SetBodyTop(ItemType.BodyTop_Shirt, Colors.White);
        Frog.SetBodyPattern(ItemType.BodyPattern_Jeans, PantsColor);
    }
}
