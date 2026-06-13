using Godot;

public partial class ScientistLabScene : GameScene
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public Camera3D Camera;

    [Export]
    public FactoryEntryDoor FactoryDoor;

    [Export]
    public FrogScientistNpc ScientistNpc;

    protected override void Initialize()
    {
        base.Initialize();
        HandInController.Instance.OnHandInClaimed += HandIn_Claimed;
        FactoryDoor.OnInteractLocked += FactoryDoor_InteractLocked;

        Camera.Current = true;
        Player.Instance.Controller.OverrideCamera = Camera;
        UpdateDoorLocked();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandIn_Claimed;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_CameraPosition();
    }

    private void Process_CameraPosition()
    {
        Camera.Position = Camera.Position.Lerp(Player.Instance.Position * 0.2f, GameTime.DeltaTime * 2f);
    }

    private void HandIn_Claimed(string id)
    {
        if (id == HandInInfo.Id)
        {
            UpdateDoorLocked();
        }
    }

    private void UpdateDoorLocked()
    {
        FactoryDoor.Locked = HandInInfo.Data.ClaimCount == 0;
    }

    private void FactoryDoor_InteractLocked()
    {
        ScientistNpc.StartLockedDoorDialogue();
    }
}
