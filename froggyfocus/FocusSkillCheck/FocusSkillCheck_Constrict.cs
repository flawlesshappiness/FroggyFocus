using Godot;
using System.Collections;

public partial class FocusSkillCheck_Constrict : FocusSkillCheck
{
    [Export]
    public Vector2I CountRange;

    [Export]
    public SkillCheckAlgae Algae;

    private int count_current;
    private int count_max;

    public override void Initialize(FocusEvent focus_event)
    {
        base.Initialize(focus_event);
        FocusEvent.Cursor.OnMoveStarted += MoveStarted;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        FocusEvent.Cursor.OnMoveStarted -= MoveStarted;
    }

    public override void Clear()
    {
        base.Clear();
        FocusCursor.MoveLock.SetLock(nameof(FocusSkillCheck_Constrict), false);
    }

    protected override IEnumerator Run()
    {
        count_max = GetDifficultyRange(CountRange);
        count_current = count_max;

        Algae.GlobalPosition = FocusEvent.Cursor.GlobalPosition;
        Algae.UpdateSize(count_max, count_max);
        Algae.StartConstrict();
        yield return null;
    }

    private void MoveStarted()
    {
        if (!IsRunning) return;

        count_current--;
        Algae.UpdateSize(count_current, count_max);

        if (count_current > 0)
        {
            Algae.Shake();
        }
        else
        {
            Algae.SetCleared();
            Clear();
        }
    }
}
