using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class FocusSkillCheck_Bombs : FocusSkillCheck
{
    [Export]
    public Vector2I BombCountRange;

    [Export]
    public Vector2 DistanceRange;

    [Export]
    public PackedScene BombPrefab;

    [Export]
    public PackedScene ProjectilePrefab;

    [Export]
    public PackedScene PsExplode;

    [Export]
    public PackedScene PsSpores;

    private List<ParticleEffectGroup> effects = new();
    private List<Bomb> bombs = new();

    private int id = 0;

    private Coroutine cr_clear;

    private class Bomb
    {
        public SkillCheckBomb BombNode { get; set; }
        public SkillCheckBombProjectile ProjectileNode { get; set; }
        public Vector3 Position { get; set; }
        public Coroutine Coroutine { get; set; }
        public float Delay { get; set; }

        public void Clear()
        {
            BombNode.QueueFree();
            ProjectileNode.QueueFree();
            Coroutine.Stop(Coroutine);
        }
    }

    public override void Clear()
    {
        base.Clear();

        bombs.ForEach(x => x.Clear());
        bombs.Clear();

        effects.ForEach(x => x.Destroy(true));
        effects.Clear();

        Coroutine.Stop(cr_clear);
    }

    protected override IEnumerator Run()
    {
        var last_angle = rng.RandfRange(0f, 360f);
        var count = BombCountRange.Range(Difficulty);
        var delay_span = 0.6f;
        var delay_per = delay_span / count;
        for (int i = 0; i < count; i++)
        {
            last_angle += rng.RandfRange(45, 180);
            var bomb = CreateBomb();
            bomb.Position = GetBombPosition(last_angle);
            bomb.Delay = i * delay_per;
            bomb.Coroutine = RunBomb(bomb);
        }

        WaitForBombs();

        yield return null;
    }

    private Coroutine WaitForBombs()
    {
        cr_clear = this.StartCoroutine(Cr, nameof(WaitForBombs));
        return cr_clear;
        IEnumerator Cr()
        {
            foreach (var bomb in bombs.ToList())
            {
                yield return bomb.Coroutine;
            }

            Clear();
        }
    }

    private Coroutine RunBomb(Bomb bomb)
    {
        return this.StartCoroutine(Cr, $"bomb_{id++}");
        IEnumerator Cr()
        {
            yield return new WaitForSeconds(bomb.Delay);

            bomb.ProjectileNode.GlobalPosition = bomb.Position;

            bomb.BombNode.RotationDegrees = Vector3.Up * rng.RandfRange(0f, 360f);
            bomb.BombNode.GlobalPosition = bomb.Position;
            bomb.BombNode.Show();

            var ps_spores = PlaySporesPS(bomb.Position);

            yield return bomb.BombNode.AnimateShow();

            bomb.BombNode.AnimateIdleFast();

            yield return new WaitForSeconds(1f);

            ps_spores.Stop(true);

            PlayExplodePS(bomb.Position);
            yield return bomb.BombNode.AnimateExplode();

            var cursor = FocusEvent.Cursor;
            yield return bomb.ProjectileNode.WaitForMoveTowardsCursor(0.5f, cursor);

            var value = FocusEvent.Target.Info.FocusValue * 0.1f;
            FocusEvent.Cursor.HurtFocusValue(value);

            yield return new WaitForSeconds(0.5f);
        }
    }

    private Bomb CreateBomb()
    {
        var bomb = new Bomb
        {
            BombNode = CreateBombNode(),
            ProjectileNode = CreateProjectileNode(),
        };

        bombs.Add(bomb);

        return bomb;
    }

    private SkillCheckBomb CreateBombNode()
    {
        var node = BombPrefab.Instantiate<SkillCheckBomb>();
        node.SetParent(this);
        return node;
    }

    private SkillCheckBombProjectile CreateProjectileNode()
    {
        var node = ProjectilePrefab.Instantiate<SkillCheckBombProjectile>();
        node.SetParent(this);
        return node;
    }

    private Vector3 GetBombPosition(float angle)
    {
        var radius_mul = rng.RandfRange(0.75f, 1f);
        var radius = Mathf.Lerp(DistanceRange.X, DistanceRange.Y, Difficulty);
        var center = FocusEvent.Target.GlobalPosition;
        var dir = Vector3.Forward.Rotated(Vector3.Up, Mathf.DegToRad(angle));
        var position = center + dir * radius * radius_mul;
        return position;
    }

    private void PlayExplodePS(Vector3 position)
    {
        var ps = PsExplode.Instantiate<ParticleEffectGroup>();
        ps.SetParent(Scene.Current);
        ps.GlobalPosition = position;
        ps.Play(true);
    }

    private ParticleEffectGroup PlaySporesPS(Vector3 position)
    {
        var ps = PsSpores.Instantiate<ParticleEffectGroup>();
        ps.SetParent(Scene.Current);
        ps.GlobalPosition = position;
        ps.Play();
        effects.Add(ps);
        return ps;
    }
}
