using Godot;
using System;

public partial class EldritchTentacleObjective : Node3D
{
    [Export]
    public string GameFlagId;

    [Export]
    public EldritchTentacle Tentacle;

    [Export]
    public Interactable Interactable;

    [Export]
    public Node3D Syringe;

    [Export]
    public FocusEvent FocusEvent;

    public bool IsCompleted => GameFlags.IsFlag(GameFlagId, 1);

    public event Action OnCompleted;

    private string DebugId => nameof(EldritchTentacleObjective) + GetInstanceId();

    private bool active_event;

    public override void _Ready()
    {
        base._Ready();
        FocusEvent.OnCompleted += FocusEventCompleted;
        FocusEvent.OnFailed += FocusEventFailed;
        Interactable.OnInteract += Interact;

        if (IsCompleted)
        {
            Complete();
        }

        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = Name;

        Debug.RegisterAction(new DebugAction
        {
            Id = DebugId,
            Category = category,
            Text = "Reset GameFlag",
            Action = v => ResetObjective()
        });
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(DebugId);
    }

    public void ResetObjective()
    {
        GameFlags.SetFlag(GameFlagId, 0);
    }

    public void Interact()
    {
        active_event = true;
        FocusEvent.StartEvent();
    }

    private void FocusEventCompleted(FocusEventCompletedResult result)
    {
        if (active_event)
        {
            GameFlags.SetFlag(GameFlagId, 1);
            Complete();
            OnCompleted?.Invoke();
        }

        active_event = false;
    }

    private void FocusEventFailed(FocusEventFailedResult result)
    {
        active_event = false;
    }

    private void Complete()
    {
        Tentacle.TriggerAsleep();
        Syringe.Hide();
        Interactable.Disable();
    }
}
