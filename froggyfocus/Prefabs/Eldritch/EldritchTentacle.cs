using FlawLizArt.Animation.StateMachine;
using Godot;
using System;
using System.Collections;

public enum EldritchTentacleStartState
{
    Curved, InWater, Curved_Idle, InPain, Idle
}

public partial class EldritchTentacle : Node3D
{
    [Export]
    public EldritchTentacleStartState StartState;

    [Export]
    public AnimationStateMachine Animation;

    [Export]
    public EldritchEye Eye;

    private static event Action<bool> OnAwakeStateChanged;

    private TriggerParameter param_awake = new("awake");
    private TriggerParameter param_asleep = new("asleep");

    public override void _Ready()
    {
        base._Ready();
        InitializeAnimations();

        OnAwakeStateChanged += AwakeStateChanged;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        OnAwakeStateChanged -= AwakeStateChanged;
    }

    private void InitializeAnimations()
    {
        var curved = Animation.CreateAnimation("Armature|curved", true);
        var curved_to_idle = Animation.CreateAnimation("Armature|curved_to_idle", false);
        var curved_to_in_water = Animation.CreateAnimation("Armature|curved_to_in_water", false);
        var idle_curved = Animation.CreateAnimation("Armature|idle_curved", true);
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var in_water = Animation.CreateAnimation("Armature|in_water", true);
        var in_water_to_idle = Animation.CreateAnimation("Armature|in_water_to_idle", false);
        var in_pain = Animation.CreateAnimation("Armature|in_pain", true);
        var in_pain_to_in_water = Animation.CreateAnimation("Armature|in_pain_to_in_water", false);

        var start = StartState switch
        {
            EldritchTentacleStartState.Curved => curved,
            EldritchTentacleStartState.InWater => in_water,
            EldritchTentacleStartState.Curved_Idle => idle_curved,
            EldritchTentacleStartState.InPain => in_pain,
            EldritchTentacleStartState.Idle => idle,
            _ => in_water
        };

        Animation.Connect(in_water, in_water_to_idle, param_awake.WhenTriggered());
        Animation.Connect(curved, curved_to_idle, param_awake.WhenTriggered());

        Animation.Connect(idle_curved, curved_to_in_water, param_asleep.WhenTriggered());
        Animation.Connect(idle_curved, curved_to_idle, param_awake.WhenTriggered());

        Animation.Connect(curved, curved_to_in_water, param_asleep.WhenTriggered());
        Animation.Connect(in_pain, in_pain_to_in_water, param_asleep.WhenTriggered());

        Animation.Connect(in_water_to_idle, idle);
        Animation.Connect(curved_to_idle, idle);

        Animation.Connect(curved_to_in_water, in_water);
        Animation.Connect(in_pain_to_in_water, in_water);

        Animation.Start(start.Node);
    }

    public void TriggerAsleep()
    {
        param_asleep.Trigger();
    }

    public void TriggerAwake()
    {
        param_awake.Trigger();
    }

    public static void SetAwakeGlobal(bool awake)
    {
        OnAwakeStateChanged?.Invoke(awake);
    }

    private void AwakeStateChanged(bool awake)
    {
        this.StartCoroutine(Cr, "awake_state");
        IEnumerator Cr()
        {
            var rng = new RandomNumberGenerator();
            yield return new WaitForSeconds(rng.RandfRange(0f, 2f));
            TriggerAwake();
        }
    }
}
