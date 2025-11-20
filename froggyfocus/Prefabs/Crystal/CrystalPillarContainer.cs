using Godot;
using System;

public partial class CrystalPillarContainer : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public CollisionShape3D Collider;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public bool IsCompleted => HandInInfo.Data.ClaimedCount > 0;

    private string DebugId => $"{nameof(CrystalEnergyContainer)}{GetInstanceId()}";

    private bool active_dialogue;

    public event Action OnCompleted;
    public event Action OnNotCompleted;

    public override void _Ready()
    {
        base._Ready();
        RegisterDebugActions();
        InitializeHandIn();
        InitializeOpen();

        HandInController.Instance.OnHandInClaimed += HandInClaimed;
        DialogueController.Instance.OnNodeEnded += DialogueNodeEnded;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        HandInController.Instance.OnHandInClaimed -= HandInClaimed;
        DialogueController.Instance.OnNodeEnded -= DialogueNodeEnded;

        Debug.RemoveActions(DebugId);
    }

    private void RegisterDebugActions()
    {
        if (HandInInfo == null) return;

        var category = HandInInfo.Id;

        Debug.RegisterAction(new DebugAction
        {
            Id = DebugId,
            Category = category,
            Text = "Set Completed",
            Action = v => { SetCompleted(true); v.Close(); }
        });

        Debug.RegisterAction(new DebugAction
        {
            Id = DebugId,
            Category = category,
            Text = "Set Uncompleted",
            Action = v => { SetCompleted(false); v.Close(); }
        });

        void SetCompleted(bool completed)
        {
            var count = completed ? 1 : 0;
            HandInInfo.Data.ClaimedCount = count;
            Data.Game.Save();

            SetOpen(!completed);
            SetInteractive(!completed);

            if (completed)
            {
                OnCompleted?.Invoke();
            }
            else
            {
                OnNotCompleted?.Invoke();
            }
        }
    }

    private void InitializeHandIn()
    {
        HandIn.InitializeData(HandInInfo);
        SetInteractive(!IsCompleted);
    }

    private void InitializeOpen()
    {
        var anim = IsCompleted ? "closed" : "open";
        AnimationPlayer.Play(anim);
    }

    public void Interact()
    {
        if (IsCompleted)
        {

        }
        else
        {
            active_dialogue = true;
            DialogueController.Instance.StartDialogue("##CRYSTAL_CONTAINER_REQUEST##");
        }
    }

    private void SetInteractive(bool interactive)
    {
        Collider.Disabled = !interactive;
    }

    private void HandInClaimed(string id)
    {
        if (id != HandInInfo.Id) return;

        SetInteractive(false);
        SetOpen(false);
        OnCompleted?.Invoke();
    }

    private void DialogueNodeEnded(string id)
    {
        if (!active_dialogue) return;

        if (id == "##CRYSTAL_CONTAINER_REQUEST##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }

        active_dialogue = false;
    }

    private void SetOpen(bool open)
    {
        var anim = open ? "open" : "close";
        AnimationPlayer.Play(anim);
    }
}
