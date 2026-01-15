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
        if (respawning) return;

        this.StartCoroutine(Cr, nameof(RespawnPlayer));
        IEnumerator Cr()
        {
            respawning = true;
            SetCamera(Player.Instance.Camera);
            PlaySplashSFX(Player.Instance.GlobalPosition);
            CreateSplashPS(Player.Instance.GlobalPosition);

            yield return new WaitForSeconds(0.5f);

            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 0.5f,
                OnTransition = () =>
                {
                    Player.Instance.SetCameraTarget();
                    Player.Instance.Respawn();
                    respawning = false;
                }
            });
        }
    }

    private void RespawnPlayerOutOfBounds()
    {
        if (respawning) return;
        respawning = true;

        TransitionView.Instance.StartTransition(new TransitionSettings
        {
            Type = TransitionType.Color,
            Color = Colors.Black,
            Duration = 0.5f,
            OnTransition = () =>
            {
                Player.Instance.Respawn();
                respawning = false;
            }
        });
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
        if (respawning) return;
        if (Player.Instance.GlobalPosition.Y < GlobalPosition.Y - 100)
        {
            RespawnPlayerOutOfBounds();
        }
    }
}
