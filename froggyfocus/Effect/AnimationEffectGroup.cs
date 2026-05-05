using Godot;
using Godot.Collections;

public partial class AnimationEffectGroup : EffectGroup
{
    [Export]
    public string Animation;

    [Export]
    public Array<AnimationPlayer> AnimationPlayers;

    protected override void OnPlay()
    {
        base.OnPlay();
        foreach (var player in AnimationPlayers)
        {
            player.Play(Animation);
        }
    }
}
