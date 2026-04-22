using FlawLizArt.Animation.StateMachine;
using Godot;
using Godot.Collections;
using System.Collections;
using System.Collections.Generic;

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

    [Export]
    public Array<string> SpeakAnimations;

    protected bool HasActiveDialogue { get; set; }
    protected AnimationState IdleState { get; set; }
    protected AnimationState DialogueState { get; set; }
    protected List<AnimationState> SpeakStates { get; set; } = new();

    protected BoolParameter param_dialogue = new BoolParameter("dialogue", false);

    private bool initialized;
    private bool dialogue_camera_in;

    public override void _Ready()
    {
        base._Ready();

        DialogueController.Instance.OnEntryStarted += DialogueNodeStarted;
        DialogueController.Instance.OnDialogueStarted += DialogueStarted;
        DialogueController.Instance.OnDialogueEnded += DialogueEnded;
        RaceController.Instance.OnCountdownStarted += Race_CountdownStarted;
        RaceController.Instance.OnRaceEnd += Race_Ended;

        InitializeAnimations();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        DialogueController.Instance.OnEntryStarted -= DialogueNodeStarted;
        DialogueController.Instance.OnDialogueStarted -= DialogueStarted;
        RaceController.Instance.OnCountdownStarted -= Race_CountdownStarted;
        RaceController.Instance.OnRaceEnd -= Race_Ended;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!initialized)
        {
            initialized = true;
            Initialize();
        }
    }

    protected virtual void Initialize() { }

    protected virtual void InitializeAnimations()
    {
        if (Animation == null) return;

        IdleState = Animation.CreateAnimation(IdleAnimation, true);

        if (!string.IsNullOrEmpty(DialogueAnimation))
        {
            DialogueState = Animation.CreateAnimation(DialogueAnimation, true);
            Animation.Connect(IdleState, DialogueState, param_dialogue.WhenTrue());
            Animation.Connect(DialogueState, IdleState, param_dialogue.WhenFalse());
        }

        InitializeSpeakAnimations();

        Animation.Start(IdleState.Node);
    }

    private void InitializeSpeakAnimations()
    {
        if (SpeakAnimations == null) return;
        if (SpeakAnimations.Count == 0) return;

        foreach (var anim in SpeakAnimations)
        {
            var state = Animation.CreateAnimation(anim, false);
            Animation.Connect(state, DialogueState ?? IdleState);
            SpeakStates.Add(state);
        }
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

            var state = SpeakStates.Random();
            if (state != null) Animation.SetCurrentState(state.Node);
        }
    }

    protected virtual void DialogueStarted(string id)
    {
        if (HasActiveDialogue)
        {
            StartDialogueCamera();
        }
    }

    protected virtual void DialogueEnded(string id)
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

    protected virtual void Race_CountdownStarted()
    {
        this.Disable();
    }

    protected virtual void Race_Ended(RaceResult result)
    {
        this.Enable();
    }
}
