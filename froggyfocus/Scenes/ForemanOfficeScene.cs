using Godot;

public partial class ForemanOfficeScene : GameScene
{
    [Export]
    public Camera3D Camera;

    protected override void Initialize()
    {
        base.Initialize();
        Camera.Current = true;
        Player.Instance.OverrideCamera = Camera;
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
