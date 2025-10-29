using FlawLizArt.Animation.StateMachine;
using Godot;
using System;

public partial class EldritchFlower : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public CollisionShape3D Collider;

    public bool IsCompleted => HandIn.GetOrCreateData(HandInInfo.Id).ClaimedCount > 0;

    private bool active_dialogue;
    private BoolParameter param_open = new BoolParameter("open", false);

    public Action OnCompleted;

    public override void _Ready()
    {
        base._Ready();
        InitializeHandIn();
        InitializeAnimations();

        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandInClaimed;
        DialogueController.Instance.OnNodeEnded -= DialogueNodeEnded;
    }

    private void InitializeHandIn()
    {
        HandIn.InitializeData(HandInInfo);

        if (IsCompleted)
        {
            DisableInteractive();
        }
    }

    private void InitializeAnimations()
    {
        var idle_open = Animation.CreateAnimation("Armature|idle_open", true);
        var idle_closed = Animation.CreateAnimation("Armature|idle_closed", true);
        var open_to_closed = Animation.CreateAnimation("Armature|open_to_closed", false);
        var closed_to_open = Animation.CreateAnimation("Armature|closed_to_open", false);

        Animation.Connect(idle_open, open_to_closed, param_open.WhenFalse());
        Animation.Connect(idle_closed, closed_to_open, param_open.WhenTrue());
        Animation.Connect(open_to_closed, idle_closed);
        Animation.Connect(closed_to_open, idle_open);

        param_open.Set(!IsCompleted);
        var start = IsCompleted ? idle_closed : idle_open;
        Animation.Start(start.Node);
    }

    public void Interact()
    {
        if (IsCompleted)
        {

        }
        else
        {
            active_dialogue = true;
            DialogueController.Instance.StartDialogue("##ELDRITCH_FLOWER_HUNGRY##");
        }
    }

    private void HandInClaimed(string id)
    {
        if (id != HandInInfo.Id) return;

        DisableInteractive();
        OnCompleted?.Invoke();
    }

    private void DisableInteractive()
    {
        Collider.Disabled = true;
        param_open.Set(false);
    }

    private void DialogueNodeEnded(string id)
    {
        if (!active_dialogue) return;

        if (id == "##ELDRITCH_FLOWER_HUNGRY##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }

        active_dialogue = false;
    }

    public void DebugSetCompleted(bool completed)
    {
        var count = completed ? 1 : 0;
        var data = HandIn.GetOrCreateData(HandInInfo.Id);
        data.ClaimedCount = count;

        param_open.Set(!completed);
    }
}
