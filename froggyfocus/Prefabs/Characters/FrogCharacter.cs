using Godot;
using System.Collections;

public partial class FrogCharacter : Character
{
    [Export]
    public Node3D Tongue;

    [Export]
    public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        base._Ready();
        Tongue.Scale = new Vector3(1, 1, 0);
    }

    public Coroutine AnimateTongueTowards(Node3D target)
    {
        var dist = Tongue.GlobalPosition.DistanceTo(target.GlobalPosition);
        return this.StartCoroutine(Cr, nameof(AnimateTongueTowards));
        IEnumerator Cr()
        {
            yield return AnimationPlayer.PlayAndWaitForAnimation("open");

            var start = Tongue.Scale.Z;
            var end = dist;
            yield return LerpEnumerator.Lerp01(0.1f, f =>
            {
                var z = Mathf.Lerp(start, end, f);
                Tongue.Scale = new Vector3(1, 1, z);
            });
        }
    }

    public Coroutine AnimageTongueBack()
    {
        return this.StartCoroutine(Cr, nameof(AnimageTongueBack));
        IEnumerator Cr()
        {
            var start = Tongue.Scale.Z;
            var end = 0;
            yield return LerpEnumerator.Lerp01(0.1f, f =>
            {
                var z = Mathf.Lerp(start, end, f);
                Tongue.Scale = new Vector3(1, 1, z);
            });

            yield return AnimationPlayer.PlayAndWaitForAnimation("close");
        }
    }
}
