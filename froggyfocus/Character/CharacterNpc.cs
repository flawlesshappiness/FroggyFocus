using FlawLizArt.Animation.StateMachine;
using Godot;
using System.Collections;

public partial class CharacterNpc : Area3D, IInteractable
{
    [Export]
    public string IdleAnimation = "Armature|idle";

    [Export]
    public string DialogueAnimation = "Armature|dialogue";

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public AudioStreamPlayer3D SfxSpeak;

    [Export]
    public Camera3D DialogueCamera;

    protected bool HasActiveDialogue { get; set; }
    protected AnimationState IdleState { get; set; }
    protected AnimationState DialogueState { get; set; }

    protected BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    private bool dialogue_camera_in;

    public override void _Ready()
    {
        base._Ready();

        DialogueController.Instance.OnNodeStarted += DialogueNodeStarted;
        DialogueController.Instance.OnDialogueStarted += DialogueStarted;
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;

        InitializeAnimations();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnNodeStarted -= DialogueNodeStarted;
        DialogueController.Instance.OnDialogueStarted -= DialogueStarted;
        DialogueController.Instance.OnDialogueEnded -= DialogueEnded;
    }

    protected virtual void InitializeAnimations()
    {
        if (Animation == null) return;

        IdleState = Animation.CreateAnimation(IdleAnimation, true);
        DialogueState = Animation.CreateAnimation(DialogueAnimation, true);

        Animation.Connect(IdleState, DialogueState, param_dialogue.WhenTrue());
        Animation.Connect(DialogueState, IdleState, param_dialogue.WhenFalse());

        Animation.Start(IdleState.Node);
    }

    public virtual void Interact()
    {

    }

    protected void StartDialogue(string id)
    {
        HasActiveDialogue = true;
        param_dialogue.Set(true);
        DialogueController.Instance.StartDialogue(id);
    }

    private void DialogueNodeStarted(string id)
    {
        if (HasActiveDialogue)
        {
            SfxSpeak?.Play();
        }
    }

    protected virtual void DialogueStarted()
    {
        if (HasActiveDialogue)
        {
            StartDialogueCamera();
        }
    }

    protected virtual void DialogueEnded()
    {
        param_dialogue.Set(false);
        HasActiveDialogue = false;
    }

    protected void StartDialogueCamera()
    {
        if (!IsInstanceValid(DialogueCamera)) return;
        if (DialogueCamera.Current) return;

        AnimateDialogueCamera(true);
    }

    protected void StopDialogueCamera()
    {
        if (!IsInstanceValid(DialogueCamera)) return;
        if (!dialogue_camera_in) return;

        AnimateDialogueCamera(false);
    }

    private Coroutine AnimateDialogueCamera(bool to_dialogue)
    {
        dialogue_camera_in = to_dialogue;

        var player_camera = Player.Instance.Camera;
        var start = to_dialogue ? player_camera.GlobalTransform : DialogueCamera.GlobalTransform;
        var end = to_dialogue ? DialogueCamera.GetParentNode3D().GlobalTransform : player_camera.GlobalTransform;

        return this.StartCoroutine(Cr, nameof(AnimateDialogueCamera));
        IEnumerator Cr()
        {
            var id = nameof(AnimateDialogueCamera) + GetInstanceId();
            if (to_dialogue)
            {
                Player.SetAllLocks(id, true);
            }

            DialogueCamera.GlobalTransform = start;
            yield return null;
            DialogueCamera.Current = true;

            var curve = Curves.EaseInOutQuad;
            yield return LerpEnumerator.Lerp01(1f, f =>
            {
                var t = curve.Evaluate(f);
                DialogueCamera.GlobalTransform = start.InterpolateWith(end, t);
            });

            if (!to_dialogue)
            {
                player_camera.Current = true;
                Player.SetAllLocks(id, false);
            }
        }
    }
}
