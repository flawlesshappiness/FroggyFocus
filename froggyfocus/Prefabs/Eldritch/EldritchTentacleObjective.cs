using FlawLizArt.FocusEvent;
using Godot;
using Godot.Collections;
using System;
using System.Linq;

public partial class EldritchTentacleObjective : Node3DScript
{
    [Export]
    public string GameFlagId;

    [Export]
    public Interactable Interactable;

    [Export]
    public Node3D Syringe;

    [Export]
    public FocusEventInfo FocusEventInfo;

    [Export]
    public Array<EldritchEye> Eyes;

    public bool IsCompleted => GameFlags.IsFlag(GameFlagId, 1);

    public event Action OnCompletedChanged;

    private bool active_event;

    public override void _Ready()
    {
        base._Ready();
        Interactable.OnInteract += Interact;
        GameFlagsController.Instance.OnFlagChanged += GameFlag_Changed;

        UpdateSyringe();
        UpdateEyes();
    }

    protected override void Initialize()
    {
        base.Initialize();
        GameScene.Instance.FocusEvent.OnEnded += FocusEventEnded;
    }

    public void Interact()
    {
        active_event = true;
        GameScene.Instance.FocusEvent.StartEvent(new FocusEvent.Settings
        {
            Id = FocusEventInfo.Id,
        });
    }

    private void FocusEventEnded(FocusEventResult result)
    {
        if (active_event && result.FocusEvent.Targets.All(x => x.IsCaught))
        {
            GameFlags.SetFlag(GameFlagId, 1);
            Data.Game.Save();
        }

        active_event = false;
    }

    private void UpdateSyringe()
    {
        Syringe.Visible = !IsCompleted;
        Interactable.SetEnabled(!IsCompleted);
    }

    private void UpdateEyes()
    {
        foreach (var eye in Eyes)
        {
            eye.SetOpen(!IsCompleted);
        }
    }

    private void GameFlag_Changed(string id, int i)
    {
        if (id == GameFlagId)
        {
            UpdateSyringe();
            UpdateEyes();
            OnCompletedChanged?.Invoke();
        }
    }
}
