using Godot;
using System.Collections;

public partial class FocusSkillCheck_EldritchSlap : FocusSkillCheck
{
    [Export]
    public EldritchTentacle Tentacle;

    [Export]
    public Node3D Pivot;

    private AnimationPlayer animation;
    private bool is_up;
    private float angle;

    public override void _Ready()
    {
        base._Ready();
        animation = Tentacle.Animation.Animator;
        Tentacle.OnSlapHit += TentacleSlapHit;
    }

    public override void Clear()
    {
        base.Clear();

        animation.Play("Armature|in_water");
        Tentacle.Hide();
    }

    protected override void Stop()
    {
        base.Stop();

        if (is_up)
        {
            animation.Play("Armature|idle_to_in_water");
        }
    }

    protected override IEnumerator Run()
    {
        StartTentacle();
        yield return null;
    }

    private Coroutine StartTentacle()
    {
        angle = rng.RandfRange(-90, 90);
        Pivot.RotationDegrees = Vector3.Up * angle;

        return this.StartCoroutine(Cr, "tentacle");
        IEnumerator Cr()
        {
            is_up = true;
            Tentacle.Show();
            yield return animation.PlayAndWaitForAnimation("Armature|in_water_to_idle");
            yield return animation.PlayAndWaitForAnimation("Armature|slap");
            is_up = false;
            yield return animation.PlayAndWaitForAnimation("Armature|idle_to_in_water");
            Clear();
        }
    }

    private void TentacleSlapHit()
    {
        FocusEvent.Cursor.HurtFocusValuePercentage(0.1f);

        if (FocusEvent.Cursor.Shield.IsShielded)
        {

        }
        else
        {
            var dir = -Vector3.Forward.Rotated(Vector3.Up, Mathf.DegToRad(angle));
            FocusEvent.Cursor.GlobalPosition += dir * 0.8f;
        }
    }
}
