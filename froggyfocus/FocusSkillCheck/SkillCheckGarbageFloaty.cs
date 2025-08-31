using Godot;
using Godot.Collections;
using System.Collections;

public partial class SkillCheckGarbageFloaty : Node3D
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Array<Node3D> Models;

    private FocusCursor cursor;
    private float radius;
    private float speed;
    private Vector3 direction;
    private bool is_running;

    public void Initialize(FocusCursor cursor, float size, float speed)
    {
        this.cursor = cursor;
        Scale = Vector3.One * size;
        radius = size * 0.5f;
        this.speed = speed;

        RandomizeModel();
    }

    public Coroutine Run(Vector3 direction, float duration)
    {
        this.direction = direction;
        return this.StartCoroutine(Cr, "start");
        IEnumerator Cr()
        {
            yield return AnimateShow();
            is_running = true;
            yield return new WaitForSeconds(duration);
            Stop();
            yield return AnimateHide();
        }
    }

    public void Stop()
    {
        is_running = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!is_running) return;

        GlobalPosition += direction * speed * GameTime.DeltaTime;
        GlobalRotationDegrees += Vector3.Up * 10f * GameTime.DeltaTime;

        if (IsInsideRadius(cursor.GlobalPosition))
        {
            var dir = GlobalPosition.DirectionTo(cursor.GlobalPosition).Normalized();
            cursor.GlobalPosition = GlobalPosition + dir * (radius + cursor.Radius);
        }
    }

    public bool IsInsideRadius(Vector3 position)
    {
        return position.DistanceTo(GlobalPosition) < radius + cursor.Radius;
    }

    private void RandomizeModel()
    {
        Models.ForEach(x => x.Hide());
        Models.PickRandom().Show();
    }

    public Coroutine AnimateShow() => Animate("show");
    public Coroutine AnimateHide() => Animate("hide");

    private Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }
}
