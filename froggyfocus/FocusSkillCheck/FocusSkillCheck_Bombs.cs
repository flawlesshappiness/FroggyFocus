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

    private List<SkillCheckBomb> bombs = new();

    private int id = 0;
    private Coroutine cr_clear;

    public override void Clear()
    {
        base.Clear();

        bombs.ForEach(x => x.Clear());
        bombs.Clear();
    }

    protected override void Stop()
    {
        base.Stop();
        Clear();
    }

    protected override IEnumerator Run()
    {
        var next_angle = rng.RandfRange(0f, 360f);
        var count = BombCountRange.Range(Difficulty);
        for (int i = 0; i < count; i++)
        {
            next_angle += rng.RandfRange(45, 180);
            var position = GetBombPosition(next_angle);
            var bomb = CreateBomb();
            bomb.GlobalPosition = position;
            bomb.StartBomb(FocusEvent);

            var delay = rng.RandfRange(0.05f, 0.15f);
            yield return new WaitForSeconds(delay);
        }

        WaitForBombs();

        yield return null;
    }

    private void WaitForBombs()
    {
        Coroutine.Stop(cr_clear);
        cr_clear = this.StartCoroutine(Cr);

        IEnumerator Cr()
        {
            foreach (var bomb in bombs.ToList())
            {
                while (bomb.Running)
                {
                    yield return null;
                }
            }

            Clear();
        }
    }

    private SkillCheckBomb CreateBomb()
    {
        var bomb = BombPrefab.Instantiate<SkillCheckBomb>();
        bomb.SetParent(this);
        bombs.Add(bomb);
        return bomb;
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
}
