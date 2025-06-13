using Godot;
using System.Collections;
using System.Collections.Generic;

public partial class FocusSkillCheck_Bombs : FocusSkillCheck
{
    [Export]
    public SkillCheckBomb BombTemplate;

    private List<Bomb> bombs = new();

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
    }

    protected override IEnumerator Run()
    {
        var last_angle = rng.RandfRange(0f, 360f);
        var count = 3;
        for (int i = 0; i < count; i++)
        {
            last_angle += rng.RandfRange(45, 180);
            var bomb = CreateBomb();
            bomb.Position = GetBombPosition(last_angle);
            bomb.Delay = i * 0.25f;
            bomb.Coroutine = RunBomb(bomb);
        }

        foreach (var bomb in bombs)
        {
            yield return bomb.Coroutine;
        }
    }

    private Coroutine RunBomb(Bomb bomb)
    {
        return Coroutine.Start(Cr);
        IEnumerator Cr()
        {
            var success = false;

            yield return new WaitForSeconds(bomb.Delay);

            // Move to position
            var start = FocusEvent.Target.GlobalPosition;
            var end = bomb.Position;
            var curve = Curves.EaseOutQuad;

            bomb.Node.GlobalPosition = start;
            bomb.Node.Show();
            bomb.Node.AnimateShow();

            yield return LerpEnumerator.Lerp01(0.5f, f =>
            {
                var t = curve.Evaluate(f);
                bomb.Node.GlobalPosition = start.Lerp(end, t);
            });

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

            // Explode
            if (success)
            {
                FocusEvent.Cursor.AdjustFocusValue(20);
                yield return bomb.Node.AnimateCollect();
            }
            else
            {
                FocusEvent.Cursor.AdjustFocusValue(-40);
                yield return bomb.Node.AnimateExplode();
            }

            yield return new WaitForSeconds(0.5f);
        }

        bool IsCursorNear()
        {
            return FocusEvent.Cursor.GlobalPosition.DistanceTo(bomb.Position) <= FocusEvent.Cursor.Radius;
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
        var radius = rng.RandfRange(1.0f, 2.0f);
        var center = FocusEvent.Target.GlobalPosition;
        var dir = Vector3.Forward.Rotated(Vector3.Up, Mathf.DegToRad(angle));
        var position = center + dir * radius;
        return position;
    }
}
