using Godot;
using System.Collections;

public partial class FocusIntroView : View
{
    public static FocusIntroView Instance => Singleton.Get<FocusIntroView>();

    [Export]
    public FrogCharacter Frog;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D TargetOrigin;

    [Export]
    public AudioStreamPlayer SfxRiff;

    private FocusCharacter current_target;

    public override void _Ready()
    {
        base._Ready();
        Frog.SetMouthOpen(true);
    }

    public IEnumerator AnimateShow()
    {
        Frog.LoadAppearance();
        Show();
        yield return AnimationPlayer.PlayAndWaitForAnimation("show2");
    }

    public IEnumerator AnimateHide()
    {
        yield return AnimationPlayer.PlayAndWaitForAnimation("hide");
        Hide();
    }

    public void PlayRiff()
    {
        SfxRiff.Play();
    }

    private void RemoveTargetCharacter()
    {
        if (current_target == null) return;

        current_target.QueueFree();
        current_target = null;
    }

    public void LoadTargetCharacter(FocusCharacterInfo info)
    {
        RemoveTargetCharacter();

        current_target = info.Scene.Instantiate<FocusCharacter>();
        current_target.SetParent(TargetOrigin);
        current_target.Position = Vector3.Zero;
        current_target.Rotation = Vector3.Zero;
        current_target.Initialize(info);
    }
}
