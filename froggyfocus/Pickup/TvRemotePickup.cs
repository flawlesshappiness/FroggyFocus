using Godot;

public partial class TvRemotePickup : Pickup
{
    [Export]
    public string PickupDialogue;

    public override void _Ready()
    {
        base._Ready();
        SetPickupEnabled(GameFlags.IsFlag(TvTravel.HasRemoteFlag, 0));
    }

    protected override void PickupCollected()
    {
        base.PickupCollected();
        DialogueController.Instance.StartDialogue(PickupDialogue);
    }
}
