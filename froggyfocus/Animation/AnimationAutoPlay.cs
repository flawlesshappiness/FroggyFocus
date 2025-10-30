using Godot;

public partial class AnimationAutoPlay : AnimationPlayer
{
    [Export]
    public string Animation;

    [Export]
    public Vector2 Delay;

    public override void _Ready()
    {
        base._Ready();

        var rng = new RandomNumberGenerator();
        var delay = Delay.Range(rng.Randf());
        Play(Animation);
        Seek(delay, update: true);
    }
}
