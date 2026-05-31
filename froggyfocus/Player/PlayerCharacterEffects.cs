using Godot;

public partial class PlayerCharacterEffects : Node3D
{
    [Export]
    public PlayerInteract Interact;

    [Export]
    public ExclamationMark ExclamationMark;

    [Export]
    public QuestionMarkEffect QuestionMark;

    [Export]
    public EffectGroupSpawner JumpChargeEffect;

    [Export]
    public GpuParticles3D PsJumpChargeMax;

    [Export]
    public AudioStreamPlayer3D SfxFocusStart;

    [Export]
    public AudioStreamPlayer3D SfxJumpCharge;

    public void SetJumpChargeMaxEmitting(bool emitting)
    {
        PsJumpChargeMax.Emitting = emitting;
    }

    public void PlayFocusStartSfx()
    {
        SfxFocusStart.Play();
    }

    public void SpawnJumpChargeEffect(float t)
    {
        JumpChargeEffect.Spawn();
        SfxJumpCharge.PitchScale = Mathf.Lerp(1.0f, 1.2f, t);
        SfxJumpCharge.Play();
    }
}
