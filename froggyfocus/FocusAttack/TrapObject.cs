using Godot;

namespace FlawLizArt.FocusEvent;

public partial class TrapObject : Node3D
{
    [Export]
    public InputPromptFocus.AnimationType InputPromptAnimationType;

    [Export]
    public Node3D SizeNode;

    [Export]
    public Vector2 SizeRange = new(0.3f, 0.9f);

    [Export]
    public float SizeAddition = 0.05f;

    protected FocusEvent FocusEvent { get; private set; }
    protected FocusCursor Cursor => FocusEvent.Cursor;
    protected int HitCount { get; private set; }
    protected int HitCountMax { get; private set; }
    protected bool Completed => HitCount <= 0;

    public void Initialize(int hit_count, FocusEvent focus_event)
    {
        FocusEvent = focus_event;
        HitCountMax = hit_count;
        HitCount = hit_count;

        UpdateSize();
        GlobalPosition = Cursor.GlobalPosition;
        SetCursorLock(true);
        FocusEventView.Instance.ShowInputPrompt("Interact", GlobalPosition.Add(z: 0.7f), InputPromptAnimationType);

        FocusEvent.OnEnded += FocusEvent_Ended;

        Started();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEvent.OnEnded -= FocusEvent_Ended;
        SetCursorLock(false);
        FocusEventView.Instance.HideInputPrompt();
    }

    protected virtual void Started()
    {

    }

    private void FocusEvent_Ended(FocusEventResult result)
    {
        QueueFree();
    }

    protected void SetCursorLock(bool locked)
    {
        var id = GetType().ToString();
        FocusCursor.MoveLock.SetLock(id, locked);
    }

    protected virtual void DecreaseCount()
    {
        HitCount--;

        if (Completed)
        {
            SetCursorLock(false);
            FocusEventView.Instance.HideInputPrompt();
        }
    }

    protected void UpdateSize() => UpdateSize(HitCount, HitCountMax);
    private void UpdateSize(float count, float count_max)
    {
        var min = SizeRange.X;
        var max = SizeRange.Y + SizeAddition * count_max;
        var t = 1f - (float)count / count_max;
        var size = Mathf.Lerp(max, min, t);
        SizeNode.Scale = Vector3.One * size;
    }
}
