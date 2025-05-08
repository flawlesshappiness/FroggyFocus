using Godot;
using System.Collections.Generic;

public partial class GameScene : Scene
{
    [Export]
    public int ActiveFocusEvents;

    [Export]
    public Node3D FocusEventParent;

    private List<FocusEvent> FocusEvents { get; set; } = new();

    public override void _Ready()
    {
        base._Ready();
        InitializeFocusEvents();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEventController.Instance.OnFocusEventDisabled -= FocusEventDisabled;
    }

    private void InitializeFocusEvents()
    {
        FocusEvents = FocusEventParent.GetNodesInChildren<FocusEvent>();
        FocusEventController.Instance.ClearEvents();
        FocusEventController.Instance.RegisterEvents(FocusEvents);
        EnableFocusEvents(ActiveFocusEvents);

        FocusEventController.Instance.OnFocusEventDisabled += FocusEventDisabled;
    }

    private void EnableFocusEvents(int count)
    {
        for (int i = 0; i < count; i++)
        {
            FocusEventController.Instance.EnableInactiveEvent(true);
        }
    }

    private void FocusEventDisabled()
    {
        FocusEventController.Instance.EnableInactiveEvent();
    }
}
