using Godot;

public partial class ItemSubViewport : PreviewSubViewport
{
    [Export]
    public AnimationPlayer AnimationPlayer_Camera;

    [Export]
    public AnimationPlayer AnimationPlayer_Target;

    public void SetHat(AppearanceHatInfo info)
    {
        var hat = SetPrefab<AppearanceHatAttachment>(info.Prefab);
        hat.SetDefaultColors();
    }

    public void SetCharacter(FocusCharacterInfo info)
    {
        var character = info.Scene.Instantiate<FocusCharacter>();
        character.Initialize(info);
        SetPreview(character);
    }

    public void SetCameraFront()
    {
        AnimationPlayer_Camera.Play("front");
    }

    public void SetCameraInventory()
    {
        AnimationPlayer_Camera.Play("inventory");
    }

    public void SetAnimationIdle()
    {
        AnimationPlayer_Target.Play("idle");
    }

    public void SetAnimationSpin()
    {
        AnimationPlayer_Target.Play("spin");
    }

    public void SetAnimationOscillate()
    {
        AnimationPlayer_Target.Play("oscillate");
    }
}
