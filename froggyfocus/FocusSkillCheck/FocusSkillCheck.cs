using Godot;
using System.Collections;

public partial class FocusSkillCheck : Node3D
{
    [Export]
    public FocusSkillCheckType Type;

    public bool IsRunning { get; set; }
    public FocusEvent FocusEvent { get; private set; }

    protected FocusTarget Target => FocusEvent.Target;
    protected float Difficulty => FocusEvent.Target.Difficulty;

    protected RandomNumberGenerator rng = new();

    public virtual void Initialize(FocusEvent focus_event)
    {
        FocusEvent = focus_event;
        FocusEvent.OnCompleted += _ => Clear();
        FocusEvent.OnFailed += _ => Clear();
        FocusEvent.OnStopped += () => Stop();
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

    protected virtual void Stop()
    {

    }

    protected virtual IEnumerator Run()
    {
        yield return null;
    }
}
