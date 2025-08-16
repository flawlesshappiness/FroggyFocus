using Godot;

public partial class SkillCheckCrystalSpike : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public void AnimateShow()
    {
        AnimationPlayer.Play("show");
    }

    public void AnimateHide()
    {
        AnimationPlayer.Play("hide");
    }
}
