using Godot;

public partial class PartnerHomeScene : GameScene
{
    [Export]
    public Camera3D Camera;

    protected override void Initialize()
    {
        base.Initialize();
        Camera.Current = true;
        Player.Instance.OverrideCamera = Camera;
    }
}
