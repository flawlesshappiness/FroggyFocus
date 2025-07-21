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
    public SkillCheckBomb BombTemplate;

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
        public SkillCheckBomb Node { get; set; }
        public Vector3 Position { get; set; }
        public Coroutine Coroutine { get; set; }
        public float Delay { get; set; }

        public void Clear()
        {
            Node.QueueFree();
            Coroutine.Stop(Coroutine);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        BombTemplate.Hide();
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
        var count = GetDifficultyInt(BombCountRange);
        for (int i = 0; i < count; i++)
        {
            last_angle += rng.RandfRange(45, 180);
            var bomb = CreateBomb();
            bomb.Position = GetBombPosition(last_angle);
            bomb.Delay = i * 0.25f;
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
            var success = false;

            yield return new WaitForSeconds(bomb.Delay);

            // Move to position

            bomb.Node.RotationDegrees = Vector3.Up * rng.RandfRange(0f, 360f);
            bomb.Node.GlobalPosition = bomb.Position;
            bomb.Node.Show();

            var ps_spores = PlaySporesPS(bomb.Position);

            yield return bomb.Node.AnimateShow();

            bomb.Node.AnimateIdle();

            // Wait for input
            var idle_fast_started = false;
            var duration = 5f;
            var time_end = GameTime.Time + duration;
            var time_idle_fast = GameTime.Time + duration * 0.75f;
            while (GameTime.Time < time_end)
            {
                if (PlayerInput.Interact.Pressed && IsCursorNear())
                {
                    success = true;
                    break;
                }

                if (!idle_fast_started && GameTime.Time > time_idle_fast)
                {
                    idle_fast_started = true;
                    bomb.Node.AnimateIdleFast();
                }

                yield return null;
            }

            ps_spores.Stop(true);

            // Explode
            if (success)
            {
                var value = FocusEvent.Target.Info.FocusValue * 0.1f;
                FocusEvent.Cursor.AdjustFocusValue(value);
                yield return bomb.Node.AnimateCollect();
            }
            else
            {
                var value = FocusEvent.Target.Info.FocusValue * -0.05f;
                PlayExplodePS(bomb.Position);
                FocusEvent.Cursor.AdjustFocusValue(value);
                yield return bomb.Node.AnimateExplode();
            }

            yield return new WaitForSeconds(0.5f);
        }

        bool IsCursorNear()
        {
            return FocusEvent.Cursor.GlobalPosition.DistanceTo(bomb.Position) <= FocusEvent.Cursor.Radius + 0.1f;
        }
    }

    private Bomb CreateBomb()
    {
        var bomb = new Bomb
        {
            Node = CreateBombNode()
        };

        bombs.Add(bomb);

        return bomb;
    }

    private SkillCheckBomb CreateBombNode()
    {
        var node = BombTemplate.Duplicate() as SkillCheckBomb;
        node.SetParent(BombTemplate.GetParent());
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
