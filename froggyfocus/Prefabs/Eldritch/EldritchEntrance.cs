using Godot;

public partial class EldritchEntrance : Area3D
{
    [Export]
    public CollisionShape3D Collider;

    [Export]
    public AnimationPlayer AnimationPlayer;

    private bool is_active;

    public override void _Ready()
    {
        base._Ready();

        BodyEntered += PlayerEntered;
    }

    public void Activate()
    {
        if (is_active) return;
        is_active = true;

        Collider.Disabled = true;
        AnimationPlayer.Play("show");
    }

    private void PlayerEntered(GodotObject go)
    {
        Data.Game.CurrentScene = nameof(EldritchScene);
        Scene.Goto(Data.Game.CurrentScene);
    }
}
