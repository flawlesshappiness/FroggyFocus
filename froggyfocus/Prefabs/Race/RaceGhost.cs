using Godot;
using System.Collections;

public partial class RaceGhost : Node3D
{
    [Export]
    public CuteFrogCharacter Character;

    public bool IsFinished { get; set; }
    private AnimationPlayer Animation_Frog { get; set; }
    private Vector3 TargetPosition { get; set; }
    private Vector3 TargetRotation { get; set; }
    private Transform3D TargetTransform { get; set; }
    private RaceGhostData GhostData { get; set; }
    private bool IsJumping { get; set; }

    public static void SetupAppearance(CuteFrogCharacter character)
    {
        character.SetBodyBase(new Color(0.2f, 0.7f, 0.9f));
        character.SetBodyTop(ItemType.BodyTop_Flames, Colors.OrangeRed);
        character.SetBodyPattern(ItemType.BodyPattern_Flames, new Color(1.0f, 0.75f, 0.05f));
        character.SetBodyEye(ItemType.BodyEye_Cute, Colors.White);
        character.SetAppearanceAttachment(ItemCategory.Face, ItemType.Face_RaceGoggles, Colors.Gray, Colors.Cyan);
        character.SetAppearanceAttachment(ItemCategory.Hat, ItemType.Hat_RaceHair, new Color(1f, 0.8f, 0f));
    }

    public override void _Ready()
    {
        base._Ready();
        Animation_Frog = Character.Animation.Animator;
        PlayIdleAnimation();
        SetupAppearance(Character);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_PositionAndRotation();
    }

    private void Process_PositionAndRotation()
    {
        var speed = 10f;
        var t = speed * GameTime.DeltaTime;
        GlobalTransform = GlobalTransform.InterpolateWith(TargetTransform, t);
    }

    public void LoadData(string id)
    {
        GhostData = RaceGhostController.Instance.GetData(id);
    }

    public void LoadData(RaceGhostInfo info)
    {
        if (info == null) return;
        GhostData = RaceGhostController.Instance.GetData(info.Id);
    }

    public void PlayGhost()
    {
        if (GhostData == null) return;

        IsFinished = false;
        this.StartCoroutine(Cr, "ghost");
        IEnumerator Cr()
        {
            var start = GameTime.Time;

            foreach (var snapshot in GhostData.Snapshots)
            {
                while ((GameTime.Time - start) < snapshot.Time)
                    yield return null;

                SetTargetTransform(snapshot.Position, snapshot.Rotation);
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
                Character.PlayJumpPS();
                Character.PlayDustStreamPS(0.4f);
            }
        }
        else if (IsJumping)
        {
            IsJumping = false;
            Character.MoveSounds.PlayLand();
            Character.PlayLandPS();
            Character.StopDustStreamPS();
        }
    }

    private void PlayIdleAnimation()
    {
        PlayAnimation("Armature|idle");
    }

    public void SetTargetTransform(Vector3 position, Vector3 rotation)
    {
        var offset = position - GlobalPosition;
        var angle = rotation.Y - GlobalRotation.Y;
        TargetTransform = GlobalTransform
            .Translated(offset)
            .RotatedLocal(Vector3.Up, angle);
    }
}
