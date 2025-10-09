using Godot;
using System.Collections;

public partial class AnimationAutoPlay : AnimationPlayer
{
    [Export]
    public string Animation;

    [Export]
    public Vector2 Delay;

    public override void _Ready()
    {
        base._Ready();

        this.StartCoroutine(Cr, "auto_animate");
        IEnumerator Cr()
        {
            var rng = new RandomNumberGenerator();
            yield return new WaitForSeconds(Delay.Range(rng.Randf()));
            Play(Animation);
        }
    }
}
