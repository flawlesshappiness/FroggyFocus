using Godot;
using System.Collections;

public partial class FocusSkillCheck : Node3D
{
    [Export]
    public FocusSkillCheckType Type;

    protected FocusEvent FocusEvent { get; set; }
    protected FocusTarget Target => FocusEvent.Target;
    protected float Difficulty => FocusEvent.Target.Info.Difficulty;

    protected RandomNumberGenerator rng = new();

    public virtual void Clear()
    {
        // Reset and hide
    }

    public IEnumerator Start(FocusEvent focus_event)
    {
        FocusEvent = focus_event;
        yield return Run();
        Clear();
    }

    protected virtual IEnumerator Run()
    {
        yield return null;
    }

    protected int GetDifficultyInt(Vector2I range) => GetDifficultyInt(range.X, range.Y);
    protected int GetDifficultyInt(int min, int max)
    {
        return (int)Mathf.Clamp(Mathf.Lerp(min, max + 1, Difficulty), min, max);
    }
}
