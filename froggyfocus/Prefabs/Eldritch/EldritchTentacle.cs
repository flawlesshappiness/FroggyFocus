using FlawLizArt.Animation.StateMachine;
using Godot;
using System;
using System.Collections;

public enum EldritchTentacleStartState
{
    Curved, InWater
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

    private BoolParameter param_awake = new("awake", false);

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
        var idle = Animation.CreateAnimation("Armature|idle", true);
        var curved = Animation.CreateAnimation("Armature|curved", true);
        var in_water = Animation.CreateAnimation("Armature|in_water", true);
        var in_water_to_idle = Animation.CreateAnimation("Armature|in_water_to_idle", false);
        var curved_to_idle = Animation.CreateAnimation("Armature|curved_to_idle", false);

        var start = StartState switch
        {
            EldritchTentacleStartState.Curved => curved,
            EldritchTentacleStartState.InWater => in_water,
            _ => in_water
        };

        Animation.Connect(in_water, in_water_to_idle, param_awake.WhenTrue());
        Animation.Connect(curved, curved_to_idle, param_awake.WhenTrue());

        Animation.Connect(in_water_to_idle, idle);
        Animation.Connect(curved_to_idle, idle);

        Animation.Start(start.Node);
    }

    public void SetAwake(bool awake)
    {
        param_awake.Set(awake);
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
            SetAwake(awake);
        }
    }
}
