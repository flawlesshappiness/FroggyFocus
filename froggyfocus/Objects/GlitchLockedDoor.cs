using Godot;

public partial class GlitchLockedDoor : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public CollisionShape3D Collider;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        base._Ready();

        InitializeHandIn();

        HandInController.Instance.OnHandInClaimed += HandInClaimed;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandInClaimed;
    }

    private void InitializeHandIn()
    {
        HandIn.InitializeData(HandInInfo);

        var data = HandIn.GetOrCreateData(HandInInfo.Id);
        var is_claimed = data.ClaimedCount > 0;
        Collider.Disabled = is_claimed;

        if (is_claimed)
        {
            AnimateOpen();
        }
    }

    public void Interact()
    {
        HandInView.Instance.ShowPopup(HandInInfo.Id);
    }

    private void AnimateOpen()
    {
        AnimationPlayer.Play("open");
    }

    private void HandInClaimed(string id)
    {
        if (id != HandInInfo.Id) return;

        Collider.Disabled = true;
        AnimateOpen();
    }
}
