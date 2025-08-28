using Godot;

public partial class FocusCursorShield : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public PackedScene PsBlock;

    public bool IsShielded { get; private set; }

    public void StartShield()
    {
        if (AnimationPlayer.IsPlaying()) return;
        IsShielded = true;
        AnimationPlayer.Stop();
        AnimationPlayer.Play("shield_instant");
    }

    public void ActivateShield()
    {
        IsShielded = true;
    }

    public void DeactivateShield()
    {
        IsShielded = false;
    }

    public void PlayBlockEffect()
    {
        var ps = ParticleEffectGroup.Instantiate(PsBlock, this);
        ps.Play(true);
    }
}
