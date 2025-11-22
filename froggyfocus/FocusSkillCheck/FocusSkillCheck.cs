using Godot;
using System.Collections;

public partial class FocusSkillCheck : Node3D
{
    [Export]
    public FocusSkillCheckType Type;

    [Export]
    public Vector2 Cooldown;

    public bool IsRunning { get; set; }
    public float TimeAvailable { get; set; }
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
        StartCooldown();
    }

    public void ResetCooldown()
    {
        TimeAvailable = GameTime.Time;
    }

    public IEnumerator Start()
    {
        IsRunning = true;
        yield return Run();
    }

    protected virtual void Stop()
    {
        // Interrupt current sequence and prepare for clear
    }

    protected virtual IEnumerator Run()
    {
        yield return null;
    }

    public bool IsAvailable()
    {
        var inactive = !IsRunning;
        var cooldown = GameTime.Time > TimeAvailable;
        return inactive && cooldown;
    }

    private void StartCooldown()
    {
        TimeAvailable = GameTime.Time + Cooldown.Range(Target.Difficulty);
    }
}
