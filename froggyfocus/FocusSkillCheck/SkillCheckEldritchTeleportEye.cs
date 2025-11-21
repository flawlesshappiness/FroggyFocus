using Godot;
using System;
using System.Collections;

public partial class SkillCheckEldritchTeleportEye : Node3D
{
    [Export]
    public FocusSkillCheck Parent;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D AimPivot;

    public event Action OnTeleport;

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_AimPivot();
    }

    public Coroutine Animate()
    {
        return this.StartCoroutine(Cr, nameof(Animate));
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("charge");
        }
    }

    private void Process_AimPivot()
    {
        var dir = GlobalPosition.DirectionTo(Parent.FocusEvent.Cursor.GlobalPosition);
        var angle = Mathf.RadToDeg(Vector3.Forward.SignedAngleTo(dir, Vector3.Up));
        AimPivot.RotationDegrees = Vector3.Zero.Set(y: angle);
    }

    public void AnimationTeleport()
    {
        OnTeleport?.Invoke();
    }

    public void Stop()
    {
        AnimationPlayer.Stop();
    }
}
