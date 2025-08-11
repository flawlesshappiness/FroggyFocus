using Godot;

public partial class PlayerMoveSounds : Node3D
{
    [Export]
    public string Id;

    [Export]
    public AudioStreamPlayer3D SfxMove;

    [Export]
    public AudioStreamPlayer3D SfxJump;

    [Export]
    public AudioStreamPlayer3D SfxLand;

    public void PlayMove()
    {
        SfxMove.Play();
    }

    public void PlayJump()
    {
        SfxJump.Play();
    }

    public void PlayLand()
    {
        SfxLand.Play();
    }
}
