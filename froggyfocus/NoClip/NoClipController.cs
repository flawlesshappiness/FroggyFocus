public partial class NoClipController : SingletonController
{
    public static NoClipController Instance => Singleton.Get<NoClipController>();
    public override string Directory => "NoClip";

    private NoClipPlayer no_clip_player;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
        InitializePlayer();
    }

    private void RegisterDebugActions()
    {
        var category = "NOCLIP";

        Debug.RegisterAction(new DebugAction
        {
            Category = category,
            Text = "Start",
            Action = v => { v.Close(); StartNoclip(); }
        });
    }

    private void InitializePlayer()
    {
        var path = $"res://{Directory}/Prefabs/no_clip_player.tscn";
        no_clip_player = GDHelper.Instantiate<NoClipPlayer>(path);
        no_clip_player.SetParent(Scene.Root);
    }

    public void StartNoclip()
    {
        var start_transform = Player.Instance.Camera.GlobalTransform;
        no_clip_player.GlobalTransform = start_transform;
        no_clip_player.Camera.Current = true;
        no_clip_player.Enabled = true;

        Player.SetAllLocks(nameof(NoClipController), true);
    }

    public void StopNoclip()
    {
        no_clip_player.Enabled = false;
        Player.Instance.GlobalPosition = no_clip_player.GlobalPosition;
        Player.Instance.SetCameraTarget();
        Player.Instance.ThirdPersonCamera.SnapToPosition();

        Player.SetAllLocks(nameof(NoClipController), false);
    }
}
