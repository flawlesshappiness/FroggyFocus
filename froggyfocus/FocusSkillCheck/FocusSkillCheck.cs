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

    protected float GetDifficultyRange(Vector2 range) => Mathf.Lerp(range.X, range.Y, Difficulty);
    protected int GetDifficultyRange(Vector2I range) => GetIntRange(range.X, range.Y, Difficulty);
    protected int GetIntRange(int min, int max, float t)
    {
        return (int)Mathf.Clamp(Mathf.Lerp(min, max + 1, t), min, max);
    }
}
