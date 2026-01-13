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
    public DifficultyStarsTexture DifficultyStars;

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

        var anim = GetShowAnimation();
        yield return AnimationPlayer.PlayAndWaitForAnimation(anim);
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

    public void LoadTarget(FocusTarget target)
    {
        LoadCharacter(target.Info);
        SetDifficultyStars(target.CharacterData.Stars);
    }

    public void LoadCharacter(FocusCharacterInfo info)
    {
        RemoveTargetCharacter();

        current_target = info.Scene.Instantiate<FocusCharacter>();
        current_target.SetParent(TargetOrigin);
        current_target.Position = Vector3.Zero;
        current_target.Rotation = Vector3.Zero;
        current_target.Initialize(info);
    }

    public void SetDifficultyStars(int count)
    {
        DifficultyStars.SetStars(count);
    }

    private string GetShowAnimation()
    {
        if (Data.Options.CutsceneTypeIndex == 1) // Fast
        {
            return "show_fast";
        }

        return "show_normal";
    }
}
