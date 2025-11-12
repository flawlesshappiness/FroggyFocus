using Godot;
using System;

public partial class CrystalEnergyContainer : Area3D, IInteractable
{
    [Export]
    public HandInInfo HandInInfo;

    [Export]
    public CollisionShape3D Collider;

    [Export]
    public Node3D Crystal;

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
        InitializeCrystal();

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
            Text = "Set On",
            Action = v => { SetCompleted(true); v.Close(); }
        });

        Debug.RegisterAction(new DebugAction
        {
            Id = DebugId,
            Category = category,
            Text = "Set Off",
            Action = v => { SetCompleted(false); v.Close(); }
        });

        void SetCompleted(bool completed)
        {
            var count = completed ? 1 : 0;
            HandInInfo.Data.ClaimedCount = count;
            Data.Game.Save();

            SetCrystalEnabled(completed);

            if (completed)
            {
                DisableInteractive();
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

        if (IsCompleted)
        {
            DisableInteractive();
        }
    }

    private void InitializeCrystal()
    {
        SetCrystalEnabled(IsCompleted);
    }

    public void Interact()
    {
        if (IsCompleted)
        {

        }
        else
        {
            active_dialogue = true;
            DialogueController.Instance.StartDialogue("##CRYSTAL_POWER_SOURCE##");
        }
    }

    private void DisableInteractive()
    {
        Collider.Disabled = true;
    }

    private void HandInClaimed(string id)
    {
        if (id != HandInInfo.Id) return;

        DisableInteractive();
        SetCrystalEnabled(true);
        OnCompleted?.Invoke();
    }

    private void DialogueNodeEnded(string id)
    {
        if (!active_dialogue) return;

        if (id == "##CRYSTAL_POWER_SOURCE##")
        {
            var data = HandIn.GetOrCreateData(HandInInfo.Id);
            HandInView.Instance.ShowPopup(data);
        }

        active_dialogue = false;
    }

    private void SetCrystalEnabled(bool enabled)
    {
        Crystal.Visible = enabled;
    }
}
