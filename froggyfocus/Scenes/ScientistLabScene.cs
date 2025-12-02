using Godot;

public partial class ScientistLabScene : GameScene
{
    [Export]
    public Camera3D Camera;

    [Export]
    public FactoryEntryDoor FactoryDoor;

    protected override void Initialize()
    {
        base.Initialize();
        Camera.Current = true;
        Player.Instance.OverrideCamera = Camera;

        FactoryDoor.Locked = true;
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
}
