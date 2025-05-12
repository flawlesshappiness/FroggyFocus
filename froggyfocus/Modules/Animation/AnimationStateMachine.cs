using FlawLizArt.Animation.StateMachine;
using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class AnimationStateMachine : BaseStateMachine
{
    [Export]
    public AnimationPlayer Animator;

    public Dictionary<string, AnimationState> Animations = new();

    public override void _Ready()
    {
        base._Ready();
        Animator.AnimationFinished += AnimationFinished;
    }

    /// <summary>
    /// Creates a StateNode, and an AnimationState using the given animation name
    /// </summary>
    /// <param name="state">The state name</param>
    /// <param name="animation">The animation name</param>
    /// <param name="looping">Should the animation loop?</param>
    public AnimationState CreateAnimation(string state, string animation, bool looping)
    {
        var animation_state = new AnimationState(animation)
        {
            Looping = looping,
            Node = CreateNode(state)
        };

        Animations.Add(state, animation_state);

        return animation_state;
    }

    /// <summary>
    /// Creates a StateNode, and an AnimationState using the given animation name
    /// </summary>
    /// <param name="animation">The animation name</param>
    /// <param name="looping">Should the animation loop?</param>
    public AnimationState CreateAnimation(string animation, bool looping)
    {
        return CreateAnimation(animation, animation, looping);
    }

    public void Connect(AnimationState start, AnimationState end, params Condition[] conditions)
    {
        Connect(start.Node, end.Node, conditions);
    }

    protected virtual void AnimationFinished(StringName animName)
    {
        TryProcessCurrentState(true);
    }

    public override void SetCurrentState(StateNode node)
    {
        base.SetCurrentState(node);

        if (Animations.TryGetValue(node.Name, out var state))
        {
            var animation = Animator.GetAnimation(state.Animation);
            animation.LoopMode = state.Looping ? Animation.LoopModeEnum.Linear : Animation.LoopModeEnum.None;
            Animator.Play(state.Animation);
        }
    }
}
