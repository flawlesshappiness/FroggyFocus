using Godot;

public partial class CollectableCoin : Node3D
{
    [Export]
    public float Radius;

    [Export]
    public AnimationPlayer AnimationPlayer;

    private bool is_picked;
    private Node3D player;

    public void Initialize(Node3D player)
    {
        this.player = player;
        is_picked = false;
        AnimationPlayer.Play("idle");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Pickup();
    }

    private void Process_Pickup()
    {
        if (is_picked) return;
        if (GlobalPosition.DistanceTo(player.GlobalPosition) < Radius)
        {
            PickUp();
        }
    }

    private void PickUp()
    {
        is_picked = true;
        AnimationPlayer.Play("pickup");
    }
}
