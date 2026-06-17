using Godot;

namespace FlawLizArt.FocusEvent;

public partial class RobotTrap : FocusAttack
{
    [Export]
    public PackedScene TrapPrefab;

    [Export]
    public int HitCount;

    protected override void Started()
    {
        base.Started();

        if (Target.Lives == 0)
        {
            Spawn();
        }
    }

    private void Spawn()
    {
        if (IsFocusTarget)
        {
            DisruptCursorFocus();
        }

        var node = TrapPrefab.Instantiate<CursorCrystal>();
        node.SetParent(Target.FocusEvent);
        node.Initialize(HitCount, Target.FocusEvent);
    }
}
