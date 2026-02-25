using Godot;
using System.Collections;

public partial class FocusSkillCheck : Node3D
{
    [Export]
    public FocusTarget Target;

    [Export]
    public FocusSkillCheckType Type;

    [Export]
    public Vector2 Cooldown;

    [Export]
    public int MinimumRarity;

    public bool IsRunning { get; set; }
    public float TimeAvailable { get; set; }
    public FocusEvent FocusEvent { get; private set; }
    protected FocusCursor Cursor => FocusEvent.Cursor;
    protected float Difficulty => Target.Difficulty;
    protected bool IsNearCursor => GlobalPosition.DistanceTo(Cursor.GlobalPosition) < Cursor.Radius;

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
        var rarity = Target.CharacterData.Stars >= MinimumRarity;
        return inactive && cooldown && rarity;
    }

    private void StartCooldown()
    {
        //TimeAvailable = GameTime.Time + Cooldown.Range(Target.Difficulty);
    }
}
