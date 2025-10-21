using Godot;
using System;
using System.Collections;

public partial class EldritchTransitionView : View
{
    public static EldritchTransitionView Instance => Get<EldritchTransitionView>();
    public override string Directory => $"{Paths.ViewDirectory}/{nameof(EldritchTransitionView)}";

    [Export]
    public AnimationPlayer AnimationPlayer;

    [Export]
    public FrogCharacter Frog;

    [Export]
    public Node3D SplashEffectNode;

    [Export]
    public PackedScene SplashEffect;

    private Action on_transition;

    public void StartTransitionShort(Action on_transition)
    {
        this.on_transition = on_transition;
        Frog.LoadAppearance();
        PlayTransition("transition_short");
    }

    public void StartTransition(Action on_transition)
    {
        this.on_transition = on_transition;
        Frog.LoadAppearance();
        PlayTransition("transition");
    }

    private void PlayTransition(string animation)
    {
        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            Show();
            Player.SetAllLocks(nameof(EldritchTransitionView), true);
            yield return AnimationPlayer.PlayAndWaitForAnimation(animation);
        }
    }

    public void EndTransition()
    {
        AnimationPlayer.Play("hide");
        Player.SetAllLocks(nameof(EldritchTransitionView), false);
    }

    public void OnTransition()
    {
        EndTransition();
        on_transition?.Invoke();
    }

    public void PlaySplashEffect()
    {
        ParticleEffectGroup.Instantiate(SplashEffect, SplashEffectNode);
    }
}
