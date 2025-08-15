using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class FocusSkillCheck_Crystal : FocusSkillCheck
{
    [Export]
    public Node3D CrystalTemplate;

    [Export]
    public Node3D ProjectileTemplate;

    private Vector3 first_projectile_direction;
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
    }

    protected override IEnumerator Run()
    {
        RunCr();
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

    private Coroutine RunCr()
    {
        var count_crystals = 2;
        var crystals = CreateCrystals(count_crystals);
        var projectile = CreateProjectile();
        var speed = 6f;
        var max_duration = 0.8f;

        first_projectile_direction = projectile.GlobalPosition.DirectionTo(crystals.First().GlobalPosition).Normalized();

        return this.StartCoroutine(Cr, nameof(RunCr));
        IEnumerator Cr()
        {
            projectile.Show();
            foreach (var crystal in crystals)
            {
                var start = projectile.GlobalPosition;
                var end = crystal.GlobalPosition;
                var duration = Mathf.Min(start.DistanceTo(end) / speed, max_duration);
                yield return LerpEnumerator.Lerp01(duration, f =>
                {
                    projectile.GlobalPosition = start.Lerp(end, f);
                });
            }

            var start_final = projectile.GlobalPosition;
            var end_final = FocusEvent.Cursor.GlobalPosition;
            var duration_final = Mathf.Min(start_final.DistanceTo(end_final) / speed, max_duration);
            yield return LerpEnumerator.Lerp01(duration_final, f =>
            {
                projectile.GlobalPosition = start_final.Lerp(FocusEvent.Cursor.GlobalPosition, f);
            });

            var hurt_value = FocusEvent.Target.Info.FocusValue * 0.2f;
            FocusEvent.Cursor.HurtFocusValue(hurt_value);

            Clear();
        }
    }

    private Vector3 RandomCrystalPosition()
    {
        return FocusEvent.Target.GetRandomPosition();
    }

    private List<Node3D> CreateCrystals(int count)
    {
        var list = new List<Node3D>();
        for (int i = 0; i < count; i++)
        {
            var position = RandomCrystalPosition();
            var node = CreateCrystal(position);
            node.Show();
            list.Add(node);
        }
        return list;
    }

    private Node3D CreateCrystal(Vector3 position)
    {
        var node = CrystalTemplate.Duplicate() as Node3D;
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
