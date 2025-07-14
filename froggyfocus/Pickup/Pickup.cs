using Godot;
using System;
using System.Collections;

public partial class Pickup : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Area3D Area;

    [Export]
    public Node3D Target;

    [Export]
    public PackedScene PsPickup;

    public event Action OnPickup;

    private bool enabled;

    public override void _Ready()
    {
        base._Ready();
        Area.BodyEntered += Area_BodyEntered;
    }

    private void Area_BodyEntered(Node3D body)
    {
        if (!enabled) return;
        enabled = false;

        OnPickup?.Invoke();

        this.StartCoroutine(Cr, "pickup");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("pickup");
            CreatePickupEffect();
            SetPickupEnabled(false);
        }
    }

    public void SetPickupEnabled(bool enabled)
    {
        this.enabled = enabled;
        Visible = enabled;

        if (enabled)
        {
            AnimationPlayer.Play("idle");
        }
    }

    private void CreatePickupEffect()
    {
        var ps = PsPickup.Instantiate<ParticleEffectGroup>();
        ps.SetParent(GetParent());
        ps.GlobalPosition = Target.GlobalPosition;
        ps.Play(true);
    }
}
