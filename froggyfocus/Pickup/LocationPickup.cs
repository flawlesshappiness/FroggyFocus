using Godot;

public partial class LocationPickup : Pickup
{
    [Export]
    public string PickupDialogue;

    [Export]
    public string LocationId;

    public override void _Ready()
    {
        base._Ready();
        var data = Location.GetOrCreateData(LocationId);
        SetPickupEnabled(!data.Unlocked);
    }

    protected override void PickupCollected()
    {
        base.PickupCollected();

        var data = Location.GetOrCreateData(LocationId);
        data.Unlocked = true;
        Data.Game.Save();

        DialogueController.Instance.StartDialogue($"##{PickupDialogue}##");
    }
}
