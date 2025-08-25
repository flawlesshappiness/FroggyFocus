using Godot;
using System.Collections;

public partial class FocusOutroView : View
{
    public static FocusOutroView Instance => Singleton.Get<FocusOutroView>();

    [Export]
    public AnimationPlayer AnimationPlayer_Frog;

    [Export]
    public AnimationPlayer AnimationPlayer_Transition;

    [Export]
    public InventoryReplacePopup InventoryReplacePopup;

    [Export]
    public SubViewport SubViewport;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public Marker3D TargetMarker;

    [Export]
    public AudioStreamPlayer SfxChordSuccess;

    [Export]
    public AudioStreamPlayer SfxChordFail;

    [Export]
    public AudioStreamPlayer SfxSwish;

    [Export]
    public GpuParticles3D PsDust;

    private FocusTarget current_target;
    private FocusCharacterInfo current_info;
    private FocusCharacter current_character;

    public override void _Ready()
    {
        base._Ready();
        ResetFrog();
    }

    private void ResetFrog()
    {
        Frog.SetLeftHandForward();
    }

    public IEnumerator EatBugSequence(bool success)
    {
        Frog.LoadAppearance();
        Show();
        SubViewport.AudioListenerEnable3D = true;
        Frog.SetHandsBack();
        yield return AnimationPlayer_Frog.PlayAndWaitForAnimation("eat_bug");

        if (!success)
        {
            current_character.Hide();
            PsDust.Emitting = true;
            SfxSwish.Play();
            yield return new WaitForSeconds(0.25f);
        }

        yield return Frog.AnimateEatTarget(current_character);
        yield return new WaitForSeconds(0.25f);

        if (success)
        {
            yield return WaitForInventory();
        }

        PlayChord(success);
        yield return new WaitForSeconds(0.25f);
        SubViewport.AudioListenerEnable3D = false;

        //TransitionView.Instance.StartTransition(Close);
        Player.Instance.SetCameraTarget();
        yield return AnimationPlayer_Transition.PlayAndWaitForAnimation("transition_hide");
        Close();
    }

    private void Close()
    {
        Hide();
        ResetFrog();
        AnimationPlayer_Transition.Play("RESET");
    }

    private IEnumerator WaitForInventory()
    {
        if (InventoryController.Instance.IsInventoryFull())
        {
            InventoryReplacePopup.SetTarget(current_target);
            yield return InventoryReplacePopup.WaitForPopup();
        }
        else
        {
            InventoryController.Instance.AddCharacter(current_target.CharacterData);
            yield return null;
        }
    }

    private void PlayChord(bool success)
    {
        if (success)
        {
            SfxChordSuccess.Play();
        }
        else
        {
            SfxChordFail.Play();
        }
    }

    private void RemoveTarget()
    {
        if (current_character == null) return;

        current_character.QueueFree();
        current_character = null;
    }

    public void CreateTarget(FocusTarget target)
    {
        RemoveTarget();

        current_target = target;
        current_info = target.Info;
        current_character = current_info.Scene.Instantiate<FocusCharacter>();
        current_character.SetParent(TargetMarker);
        current_character.Position = Vector3.Zero;
        current_character.Rotation = Vector3.Zero;
        current_character.Scale = Vector3.One * 0.3f;
        current_character.Initialize(current_info);
    }
}
