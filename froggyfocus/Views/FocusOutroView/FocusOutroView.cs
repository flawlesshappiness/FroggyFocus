using Godot;
using System.Collections;

public partial class FocusOutroView : View
{
    public static FocusOutroView Instance => Singleton.Get<FocusOutroView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

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

    private FocusCharacterInfo current_target_info;
    private Node3D current_target;

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
        Show();
        SubViewport.AudioListenerEnable3D = true;
        Frog.SetHandsBack();
        yield return AnimationPlayer.PlayAndWaitForAnimation("eat_bug");
        if (!success)
        {
            current_target.Hide();
            PsDust.Emitting = true;
            SfxSwish.Play();
            yield return new WaitForSeconds(0.25f);
        }
        yield return Frog.AnimateEatTarget(current_target);
        yield return new WaitForSeconds(0.25f);
        yield return WaitForInventory();
        PlayChord(success);
        yield return new WaitForSeconds(0.25f);
        SubViewport.AudioListenerEnable3D = false;
        Hide();

        ResetFrog();
    }

    private IEnumerator WaitForInventory()
    {
        if (InventoryController.Instance.IsInventoryFull())
        {
            yield return InventoryReplacePopup.WaitForReplace(current_target_info);
        }
        else
        {
            InventoryController.Instance.AddCharacter(current_target_info);
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
        if (current_target == null) return;

        current_target.QueueFree();
        current_target = null;
    }

    public void CreateTarget(FocusCharacterInfo info)
    {
        RemoveTarget();

        current_target_info = info;
        current_target = info.Scene.Instantiate<Node3D>();
        current_target.SetParent(TargetMarker);
        current_target.Position = Vector3.Zero;
        current_target.Rotation = Vector3.Zero;
        current_target.Scale = Vector3.One * 0.3f;
    }
}
