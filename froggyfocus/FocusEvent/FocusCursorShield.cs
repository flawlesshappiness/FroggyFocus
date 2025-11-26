using Godot;

public partial class FocusCursorShield : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

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

    public void SetShieldOn(bool on)
    {
        if (IsShielded == on) return;
        IsShielded = on;

        var anim = on ? "shield_on" : "shield_off";
        AnimationPlayer.Play(anim);
    }

    public void PlayBlockAnimation()
    {
        AnimationPlayer.Play("shield_block");
    }
}
