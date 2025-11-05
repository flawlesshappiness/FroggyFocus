using Godot;

public partial class CrystalDrill : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    public void SetAnimating(bool animating)
    {
        var anim = animating ? "spin" : "stop";
        AnimationPlayer.Play(anim);
    }

    public void StartAnimating() => SetAnimating(true);
    public void StopAnimating() => SetAnimating(false);
}
