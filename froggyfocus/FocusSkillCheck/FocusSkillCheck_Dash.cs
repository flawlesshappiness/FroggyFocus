using Godot;
using System.Collections;

public partial class FocusSkillCheck_Dash : FocusSkillCheck
{
    [Export]
    public Vector2I DashCountRange;

    [Export]
    public Vector2 TelegraphSpeedRange;

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public Node3D RotationNode;

    [Export]
    public AudioStreamPlayer SfxMove;

    public override void _Ready()
    {
        base._Ready();
    }

    public override void Clear()
    {
        base.Clear();
    }

    protected override IEnumerator Run()
    {
        yield return base.Run();

        AnimationPlayer.SpeedScale = Mathf.Lerp(TelegraphSpeedRange.X, TelegraphSpeedRange.Y, Difficulty);

        var count = GetDifficultyInt(DashCountRange);
        for (int i = 0; i < count; i++)
        {
            GlobalPosition = FocusEvent.Target.GlobalPosition;

            var position = FocusEvent.Target.GetClampedPosition();
            var dir = FocusEvent.Target.GlobalPosition.DirectionTo(position);
            var angle = Mathf.RadToDeg(Vector3.Forward.SignedAngleTo(dir, Vector3.Up));
            RotationNode.GlobalRotationDegrees = new Vector3(0, angle, 0);
            FocusEvent.Target.Character.StartFacingDirection(dir);
            yield return AnimationPlayer.PlayAndWaitForAnimation("telegraph");

            SfxMove.Play();

            var curve = Curves.EaseOutQuad;
            var start = FocusEvent.Target.GlobalPosition;
            yield return LerpEnumerator.Lerp01(0.25f, f =>
            {
                var t = curve.Evaluate(f);
                FocusEvent.Target.GlobalPosition = start.Lerp(position, t);
            });
        }

        Clear();
    }
}
