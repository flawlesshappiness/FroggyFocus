using Godot;
using System.Collections;

public partial class FocusSkillCheck : Node3D
{
    [Export]
    public FocusSkillCheckType Type;

    public bool IsRunning { get; set; }

    protected FocusEvent FocusEvent { get; set; }
    protected FocusTarget Target => FocusEvent.Target;
    protected float Difficulty => FocusEvent.Target.Info.Difficulty;

    protected RandomNumberGenerator rng = new();

    public void Initialize(FocusEvent focus_event)
    {
        FocusEvent = focus_event;
        FocusEvent.OnCompleted += _ => Clear();
        FocusEvent.OnFailed += _ => Clear();
    }

    public virtual void Clear()
    {
        // Reset and hide
        IsRunning = false;
    }

    public IEnumerator Start()
    {
        IsRunning = true;
        yield return Run();
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
