using Godot;

public partial class FocusCircle : Node
{
    [Export]
    public Node3D FillNode;

    [Export]
    public Node3D Eye;

    [Export]
    public AnimationPlayer AnimationPlayer_Gain;

    [Export]
    public AnimationPlayer AnimationPlayer_Hurt;

    [Export]
    public AnimationPlayer AnimationPlayer_Show;

    [Export]
    public AudioStreamPlayer SfxFocusGain;

    [Export]
    public AudioStreamPlayer SfxFocusMax;

    private bool is_visible = true;

    public override void _Ready()
    {
        base._Ready();
        Eye.Hide();
    }

    public void AnimateBounce(bool is_max)
    {
        AnimationPlayer_Gain.Stop();
        AnimationPlayer_Gain.Play("BounceIn");

        if (is_max)
        {
            SfxFocusMax.Play();
        }
    }

    public void AnimateHurt()
    {
        AnimationPlayer_Hurt.Play("hurt");
    }

    public void SetFill(float t)
    {
        FillNode.Scale = Vector3.One * Mathf.Clamp(t, 0.01f, 1.0f);
        UpdatePitch(t);
        SetVisible(t > 0.0f);
    }

    private void UpdatePitch(float t)
    {
        var pitch_min = 0.5f;
        var pitch_max = 1.5f;
        SfxFocusGain.PitchScale = Mathf.Lerp(pitch_min, pitch_max, t);
    }

    public void SetEyeVisible(bool visible)
    {
        Eye.Visible = visible;
    }

    public void SetVisible(bool visible)
    {
        if (is_visible == visible) return;
        is_visible = visible;

        if (visible)
        {
            AnimationPlayer_Show.Play("show");
        }
        else
        {
            AnimationPlayer_Show.Play("hide");
        }
    }
}
