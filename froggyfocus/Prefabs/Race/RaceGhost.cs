using Godot;
using System.Collections;

public partial class RaceGhost : Node3D
{
    [Export]
    public FrogCharacter Character;

    public bool IsFinished { get; set; }
    private AnimationPlayer Animation_Frog { get; set; }
    private Vector3 TargetPosition { get; set; }
    private Vector3 TargetRotation { get; set; }
    private RaceGhostData GhostData { get; set; }
    private bool IsJumping { get; set; }

    public override void _Ready()
    {
        base._Ready();
        Animation_Frog = Character.Animation.Animator;
        PlayIdleAnimation();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_PositionAndRotation();
    }

    private void Process_PositionAndRotation()
    {
        var speed = 6f;
        GlobalPosition = GlobalPosition.Lerp(TargetPosition, speed * GameTime.DeltaTime);
        GlobalRotation = GlobalRotation.Lerp(TargetRotation, speed * GameTime.DeltaTime);
    }

    public void LoadData(string id)
    {
        GhostData = RaceGhostController.Instance.GetData(id);
    }

    public void PlayGhost()
    {
        IsFinished = false;
        this.StartCoroutine(Cr, "ghost");
        IEnumerator Cr()
        {
            var start = GameTime.Time;

            foreach (var snapshot in GhostData.Snapshots)
            {
                while ((GameTime.Time - start) < snapshot.Time)
                    yield return null;

                SetTargetPosition(snapshot.Position);
                SetTargetRotation(snapshot.Rotation);
                PlayAnimation(snapshot.Animation);
            }

            PlayIdleAnimation();
            IsFinished = true;
        }
    }

    private void PlayAnimation(string animation)
    {
        if (string.IsNullOrEmpty(animation)) return;
        Animation_Frog.Play(animation);

        if (animation.ToLower().Contains("jump"))
        {
            if (!IsJumping)
            {
                IsJumping = true;
                Character.MoveSounds.PlayJump();
            }
        }
        else if (IsJumping)
        {
            IsJumping = false;
            Character.MoveSounds.PlayLand();
        }
    }

    private void PlayIdleAnimation()
    {
        PlayAnimation("Armature|idle");
    }

    public void SetTargetPosition(Vector3 position)
    {
        TargetPosition = position;
    }

    public void SetTargetRotation(Vector3 rotation)
    {
        TargetRotation = rotation;
    }
}
