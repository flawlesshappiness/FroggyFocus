using Godot;

public partial class EldritchEntrance : Area3D
{
    [Export]
    public CollisionShape3D Collider;

    [Export]
    public AnimationPlayer AnimationPlayer;

    private bool is_active;

    public void Activate()
    {
        if (is_active) return;

        Collider.Disabled = true;
        AnimationPlayer.Play("show");
    }
}
