using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class FocusSkillCheck_Crystal : FocusSkillCheck
{
    [Export]
    public Vector2 DelayRange;

    [Export]
    public Vector2 CrystalDistanceRange;

    [Export]
    public Vector2 ProjectileSpeedRange;

    [Export]
    public Node3D CrystalTemplate;

    [Export]
    public Node3D ProjectileTemplate;

    [Export]
    public AudioStreamPlayer3D SfxShoot;

    private bool recoil;
    private Vector3 first_projectile_direction;
    private Coroutine cr_run;
    private List<Node3D> created_objects = new();

    public override void _Ready()
    {
        base._Ready();
        CrystalTemplate.Hide();
        ProjectileTemplate.Hide();
    }

    public override void Clear()
    {
        base.Clear();
        created_objects.ForEach(x => x.QueueFree());
        created_objects.Clear();
        Coroutine.Stop(cr_run);
    }

    protected override IEnumerator Run()
    {
        cr_run = RunCr();
        yield return WaitForRecoilCr();
        yield return RecoilTargetCr();
    }

    private IEnumerator RecoilTargetCr()
    {
        var start = Target.GlobalPosition;
        var end = start - first_projectile_direction * 0.5f;
        var curve = Curves.EaseOutQuad;
        yield return LerpEnumerator.Lerp01(0.25f, f =>
        {
            Target.GlobalPosition = start.Lerp(end, curve.Evaluate(f));
        });
    }

    private IEnumerator WaitForRecoilCr()
    {
        recoil = false;
        while (!recoil) yield return null;
    }

    private Coroutine RunCr()
    {
        var count_crystals = 1;
        var crystals = CreateCrystals(count_crystals);
        var projectile = CreateProjectile();
        var speed = ProjectileSpeedRange.Range(Difficulty);
        var max_duration = 0.8f;

        first_projectile_direction = projectile.GlobalPosition.DirectionTo(crystals.First().GlobalPosition).Normalized();

        return this.StartCoroutine(Cr, nameof(RunCr));
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(DelayRange.Range(Difficulty));

            recoil = true;
            projectile.Show();
            SfxShoot.Play();

            foreach (var crystal in crystals)
            {
                var start = projectile.GlobalPosition;
                var end = crystal.GlobalPosition;
                var duration = Mathf.Min(start.DistanceTo(end) / speed, max_duration);
                yield return LerpEnumerator.Lerp01(duration, f =>
                {
                    projectile.GlobalPosition = start.Lerp(end, f);
                });

                crystal.AnimateHide();
            }

            var start_final = projectile.GlobalPosition;
            var end_final = FocusEvent.Cursor.GlobalPosition;
            var duration_final = Mathf.Min(start_final.DistanceTo(end_final) / speed, max_duration);
            yield return LerpEnumerator.Lerp01(duration_final, f =>
            {
                projectile.GlobalPosition = start_final.Lerp(FocusEvent.Cursor.GlobalPosition, f);
            });

            FocusEvent.Cursor.HurtFocusValuePercentage(0.2f);

            Clear();
        }
    }

    private Vector3 RandomCrystalPosition(Vector3 center, float distance)
    {
        var position = Target.GetRandomPosition();
        return center + center.DirectionTo(position).Normalized() * distance;
    }

    private List<SkillCheckCrystalSpike> CreateCrystals(int count)
    {
        var list = new List<SkillCheckCrystalSpike>();
        var position = Target.GlobalPosition;
        var distance = CrystalDistanceRange.Range(Difficulty);
        for (int i = 0; i < count; i++)
        {
            position = RandomCrystalPosition(position, distance);
            var node = CreateCrystal(position);
            node.Show();
            node.AnimateShow();
            list.Add(node);
        }
        return list;
    }

    private SkillCheckCrystalSpike CreateCrystal(Vector3 position)
    {
        var node = CrystalTemplate.Duplicate() as SkillCheckCrystalSpike;
        node.SetParent(CrystalTemplate.GetParent());
        node.GlobalPosition = position;
        created_objects.Add(node);
        return node;
    }

    private Node3D CreateProjectile()
    {
        var node = ProjectileTemplate.Duplicate() as Node3D;
        node.SetParent(ProjectileTemplate.GetParent());
        node.GlobalPosition = FocusEvent.Target.GlobalPosition;
        created_objects.Add(node);
        return node;
    }
}
