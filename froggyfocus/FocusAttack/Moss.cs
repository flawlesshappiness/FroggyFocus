using Godot;
using System.Collections;

namespace FlawLizArt.FocusEvent;

public partial class Moss : FocusAttack
{
    [Export]
    public AudioStreamPlayer SfxCreateBush;

    [Export]
    public PackedScene MossBushPrefab;

    private Coroutine cr_run;

    protected override void Started()
    {
        base.Started();
        Run();
    }

    protected override void Stopped()
    {
        base.Stopped();
        Coroutine.Stop(cr_run);
    }

    protected override void Caught()
    {
        base.Caught();

        Coroutine.Stop(cr_run);
    }

    private void Run()
    {
        cr_run = this.StartCoroutine(Cr, "run");
        IEnumerator Cr()
        {
            var cooldown = new Vector2(10f, 20f);
            var cooldown_mul = Mathf.Lerp(1f, 0.6f, Target.Difficulty);

            yield return new WaitForSeconds(rng.RandfRange(2f, 4f));
            while (true)
            {
                StartState();

                if (IsFocusTarget)
                {
                    HurtFocusValue(0.1f);
                    DisruptCursorFocus();
                }

                Target.Animate_Exclamation();
                yield return Target.Animate_DigDown();

                var bush = CreateMossBushes();
                Target.GlobalPosition = bush.GlobalPosition;
                SfxCreateBush.Play();

                while (!bush.Triggered)
                {
                    yield return null;
                }

                yield return Target.Animate_DigUp();

                EndState();

                yield return new WaitForSeconds(rng.RandfRange(cooldown.X, cooldown.Y) * cooldown_mul);
            }
        }
    }

    private MossBush CreateMossBushes()
    {
        var count = 3;
        MossBush first = null;
        for (int i = 0; i < count; i++)
        {
            var bush = CreateMossBush();
            var angle = rng.RandfRange(0f, 360f);
            var dir = Vector3.Forward.Rotated(Vector3.Up, Mathf.DegToRad(angle));
            var position = Target.GlobalPosition + dir * rng.RandfRange(3f, 4f);
            bush.GlobalPosition = Target.GetApproximatePosition(position);
            bush.Initialize(Target.FocusEvent);

            first = i == 0 ? bush : first;
        }

        return first;
    }

    private MossBush CreateMossBush()
    {
        var node = MossBushPrefab.Instantiate<MossBush>();
        node.SetParent(Target.FocusEvent);
        return node;
    }
}
