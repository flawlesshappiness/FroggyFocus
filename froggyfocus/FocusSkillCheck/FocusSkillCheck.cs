using Godot;
using System.Collections;

public partial class FocusSkillCheck : Node3D
{
    [Export]
    public FocusSkillCheckType Type;

    protected FocusEvent FocusEvent { get; set; }

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
}
