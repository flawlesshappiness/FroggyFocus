using Godot;
using System.Collections;

public partial class RespawnArea : Area3D
{
    [Export]
    public AudioStreamPlayer3D SfxImpact;

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
            CameraController.Instance.Target = null;
            SfxImpact.GlobalPosition = Player.Instance.GlobalPosition;
            SfxImpact.Play();
            yield return new WaitForSeconds(1f);
            Player.Instance.SetCameraTarget();
            Player.Instance.Respawn();
            respawning = false;
        }
    }
}
