using Godot;
using System.Collections;

public partial class FocusOutroView : View
{
    public static FocusOutroView Instance => Singleton.Get<FocusOutroView>();

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public SubViewport SubViewport;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public Marker3D TargetMarker;

    [Export]
    public AudioStreamPlayer SfxChord;

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

    public IEnumerator EatBugSequence()
    {
        Show();
        SubViewport.AudioListenerEnable3D = true;
        Frog.SetHandsBack();
        yield return AnimationPlayer.PlayAndWaitForAnimation("eat_bug");
        yield return Frog.AnimateEatTarget(current_target);
        yield return new WaitForSeconds(0.25f);
        SfxChord.Play();
        yield return new WaitForSeconds(0.25f);
        SubViewport.AudioListenerEnable3D = false;
        Hide();

        ResetFrog();
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

        current_target = info.Scene.Instantiate<Node3D>();
        current_target.SetParent(TargetMarker);
        current_target.Position = Vector3.Zero;
        current_target.Rotation = Vector3.Zero;
        current_target.Scale = Vector3.One * 0.3f;
    }
}
