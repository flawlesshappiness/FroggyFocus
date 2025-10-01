using Godot;
using System.Collections;

public partial class SkillCheckBomb : Node3D
{
    [Export]
    public string AnimShow = "show";

    [Export]
    public string AnimIdle = "idle";

    [Export]
    public string AnimExplode = "explode";

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public PackedScene ProjectilePrefab;

    [Export]
    public ParticleEffectGroup PsIdle;

    [Export]
    public ParticleEffectGroup PsExplode;

    public Coroutine AnimateShow() => Animate(AnimShow);
    public Coroutine AnimateIdle() => Animate(AnimIdle);
    public Coroutine AnimateExplode() => Animate(AnimExplode);

    public bool Running { get; private set; }

    private FocusEvent focus_event;
    private SkillCheckProjectile projectile;

    public void Clear()
    {
        if (IsInstanceValid(projectile))
        {
            projectile.QueueFree();
            projectile = null;
        }

        QueueFree();
    }

    private Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr);
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }

    public void StartBomb(FocusEvent focus_event)
    {
        this.focus_event = focus_event;
        Running = true;
        this.StartCoroutine(Cr, "bomb");
        IEnumerator Cr()
        {
            yield return AnimateShow();
            AnimateIdle();
            yield return new WaitForSeconds(1f);
            yield return AnimateExplode();
            PlayExplodePS();

            projectile = SpawnProjectile();
            yield return projectile.StartProjectile(new SkillCheckProjectile.Settings { FocusEvent = focus_event });

            while (!projectile.IsHit)
            {
                yield return null;
            }

            Running = false;
        }
    }

    private SkillCheckProjectile SpawnProjectile()
    {
        var p = ProjectilePrefab.Instantiate<SkillCheckProjectile>();
        p.SetParent(this);
        return p;
    }

    private void PlayExplodePS()
    {
        PsIdle?.SetParent(focus_event);
        PsIdle?.Stop(destroy: true);

        PsExplode?.SetParent(focus_event);
        PsExplode?.Play(destroy: true);
    }
}
