using Godot;
using System.Collections;

public partial class SkillCheckOilClone : Character
{
    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D ModelFlying;

    [Export]
    public Node3D ModelWorm;

    public bool IsTarget { get; set; }

    private FocusTarget target;
    private bool moving;
    private bool close_position;
    private Vector3 next_position;
    private Coroutine cr_move;

    public void Initialize(FocusTarget target)
    {
        this.target = target;

        if (target.Info.Tags.Contains(FocusCharacterTag.Flying))
        {
            SetFlying();
        }
        else
        {
            SetWorm();
        }
    }

    public void StopMoving()
    {
        moving = false;
        Coroutine.Stop(cr_move);
        StopFacingDirection();
        cr_move = null;
    }

    public void StartMoving()
    {
        StopMoving();
        moving = true;
        cr_move = this.StartCoroutine(Cr, "moving");
        IEnumerator Cr()
        {
            while (true)
            {
                next_position = GetNextPosition();
                StartFacingPosition(next_position);

                while (GlobalPosition.DistanceTo(next_position) > 0.1f)
                {
                    var dir = GlobalPosition.DirectionTo(next_position).Normalized();
                    var speed = target.UpdatedMoveSpeed * 0.7f;
                    var velocity = dir * speed * GameTime.DeltaTime;
                    GlobalPosition += velocity;
                    yield return null;
                }
            }
        }
    }

    private Vector3 GetNextPosition()
    {
        close_position = !close_position;

        if (close_position || IsTarget)
        {
            return target.GetNextPosition();
        }
        else
        {
            return target.GetRandomPosition();
        }
    }

    public Coroutine AnimateShow() => Animate("show");
    public Coroutine AnimateHide() => Animate("hide");

    public Coroutine Animate(string animation)
    {
        return this.StartCoroutine(Cr, "animate");
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }

    public void SetWorm()
    {
        ModelFlying.Hide();
        ModelWorm.Show();
    }

    public void SetFlying()
    {
        ModelWorm.Hide();
        ModelFlying.Show();
    }
}
