using Godot;
using System.Collections;

public static class AnimationPlayerExtensions
{
    public static Coroutine PlayAndWaitForAnimation(this AnimationPlayer player, string animation)
    {
        player.Play(animation);
        player.AnimationFinished += AnimationFinished;

        bool animation_finished = false;
        var id_cr = $"{nameof(PlayAndWaitForAnimation)}_{player.GetInstanceId()}_{GameTime.UnscaledTime}";
        return Coroutine.Start(WaitForAnimationCr(), id_cr, player)
            .SetRunWhilePaused();

        IEnumerator WaitForAnimationCr()
        {
            while (!animation_finished)
            {
                yield return null;
            }
        }

        void AnimationFinished(StringName animation_name)
        {
            if (animation_name.ToString() == animation)
            {
                animation_finished = true;
                player.AnimationFinished -= AnimationFinished;
            }
        }
    }

    /// <summary>
    /// Stops current animation, if same as argument, then plays animation
    /// </summary>
    public static void Replay(this AnimationPlayer player, string animation, double custom_blend = -1, float custom_speed = 1f, bool from_end = false, bool keep_state = false)
    {
        if (player.CurrentAnimation == animation)
        {
            player.Stop(keep_state);
        }

        player.Play(animation,
            customBlend: custom_blend,
            customSpeed: custom_speed,
            fromEnd: from_end);
    }
}
