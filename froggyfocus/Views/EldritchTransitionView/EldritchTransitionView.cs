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

    public void StartTransitionEnter()
    {
        WeatherController.Instance.StopWeather();

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            Show();
            yield return AnimationPlayer.PlayAndWaitForAnimation("splash");
            var scene = Scene.Goto<EldritchTransitionScene>();
            Hide();
            yield return scene.StartTransition_Enter();
            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 1.0f,
                OnTransition = () =>
                {
                    Scene.Goto<EldritchScene>();
                }
            });
        }
    }

    public void StartTransitionExit()
    {
        WeatherController.Instance.StopWeather();
        var scene = Scene.Goto<EldritchTransitionScene>();

        this.StartCoroutine(Cr, "transition");
        IEnumerator Cr()
        {
            yield return scene.StartTransition_Exit();
            TransitionView.Instance.StartTransition(new TransitionSettings
            {
                Type = TransitionType.Color,
                Color = Colors.Black,
                Duration = 2.0f,
                OnTransition = () =>
                {
                    Scene.Goto<SwampScene>();
                }
            });
        }
    }

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
