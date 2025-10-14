using Godot;

public partial class EldritchTentacleWall : Node3D
{
    [Export]
    public EldritchTentacleObjective Objective;

    [Export]
    public EldritchTentacle Tentacle;

    [Export]
    public CollisionShape3D Collider;

    public override void _Ready()
    {
        base._Ready();

        if (Objective == null) return;
        Objective.OnCompleted += ObjectiveCompleted;

        if (Objective.IsCompleted)
        {
            Complete();
        }
    }

    private void ObjectiveCompleted()
    {
        Complete();
    }

    private void Complete()
    {
        Collider.Disabled = true;
        Tentacle.TriggerAsleep();
    }
}
