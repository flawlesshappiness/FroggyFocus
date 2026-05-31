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

    public override void _Ready()
    {
        base._Ready();
        BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        CheckPlayerOutOfBounds();
    }

    private void OnBodyEntered(GodotObject go)
    {
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        if (Player.Instance.Controller.IsRespawning) return;

        this.StartCoroutine(Cr, nameof(RespawnPlayer));
        IEnumerator Cr()
        {
            SetCamera(Player.Instance.Camera);
            PlaySplashSFX(Player.Instance.GlobalPosition);
            CreateSplashPS(Player.Instance.GlobalPosition);

            yield return new WaitForSeconds(0.5f);

            Player.Instance.Respawn();
        }
    }

    private void RespawnPlayerOutOfBounds()
    {
        Player.Instance.Respawn();
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

    private void CheckPlayerOutOfBounds()
    {
        if (Player.Instance == null) return;
        if (Player.Instance.Controller.IsRespawning) return;
        if (Player.Instance.GlobalPosition.Y < GlobalPosition.Y - 100)
        {
            RespawnPlayerOutOfBounds();
        }
    }
}
