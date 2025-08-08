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

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (PlayerInput.Interact.Released)
        {
            DecreaseCount();
        }
    }

    private void SetLocks(bool locked)
    {
        var id = nameof(FocusSkillCheck_Constrict);
        FocusCursor.MoveLock.SetLock(id, locked);
        FocusCursor.ShieldLock.SetLock(id, locked);
    }

    public override void Clear()
    {
        base.Clear();
        Algae.Clear();
        SetLocks(false);
    }

    protected override IEnumerator Run()
    {
        SetLocks(true);

        count_max = CountRange.Range(Difficulty);
        count_current = count_max;

        Algae.GlobalPosition = FocusEvent.Cursor.GlobalPosition;
        Algae.UpdateSize(count_max, count_max);
        Algae.StartConstrict();
        yield return null;
    }

    private void DecreaseCount()
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
            Algae.Shake();
            Clear();
        }
    }
}
