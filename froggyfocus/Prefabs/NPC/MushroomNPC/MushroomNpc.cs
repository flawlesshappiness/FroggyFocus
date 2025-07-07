using Godot;

public partial class MushroomNpc : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    public override void _Ready()
    {
        base._Ready();
        HandIn.InitializeData(HandInInfo);
        HandInController.Instance.OnHandInClaimed += HandInClaimed;
    }

    public void Interact()
    {
        var data = HandIn.GetOrCreateData(HandInInfo.Id);
        HandInView.Instance.ShowPopup(data);
    }

    private void HandInClaimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            HandIn.ResetData(HandInInfo);
        }
    }
}
