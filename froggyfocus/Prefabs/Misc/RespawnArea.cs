using Godot;
using System.Collections;

public partial class RespawnArea : Area3D
{
    [Export]
    public float MinCameraHeight;

    [Export]
    public Camera3D Camera;

    [Export]
    public AudioStreamPlayer3D SfxImpact;

    [Export]
    public PackedScene PsSplashPrefab;

    private bool respawning;

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(GodotObject go)
    {
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        if (respawning) return;

        this.StartCoroutine(Cr, nameof(RespawnPlayer));
        IEnumerator Cr()
        {
            respawning = true;
            SetCamera(Player.Instance.Camera);
            PlaySplashSFX(Player.Instance.GlobalPosition);
            CreateSplashPS(Player.Instance.GlobalPosition);

            yield return new WaitForSeconds(1f);
            Player.Instance.SetCameraTarget();
            Player.Instance.Respawn();
            respawning = false;
        }
    }

    private void SetCamera(Camera3D other)
    {
        Camera.GlobalTransform = other.GlobalTransform;
        Camera.Current = true;

        var cam_pos = Camera.GlobalPosition;
        var y = Mathf.Max(Camera.GlobalPosition.Y, GlobalPosition.Y + MinCameraHeight);
        Camera.GlobalPosition = new Vector3(cam_pos.X, y, cam_pos.Z);
    }

    private void PlaySplashSFX(Vector3 position)
    {
        SfxImpact.GlobalPosition = Player.Instance.GlobalPosition;
        SfxImpact.Play();
    }

    private void CreateSplashPS(Vector3 position)
    {
        var ps = PsSplashPrefab.Instantiate<ParticleEffectGroup>();
        ps.SetParent(this);
        ps.GlobalPosition = Player.Instance.GlobalPosition;
        ps.Position = ps.Position.Set(y: 0);
        ps.Play(destroy: true);
    }
}
