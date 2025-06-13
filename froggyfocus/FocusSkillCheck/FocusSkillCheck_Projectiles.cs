using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class FocusSkillCheck_Projectiles : FocusSkillCheck
{
    [Export]
    public Node3D TargetTemplate;

    [Export]
    public Node3D MoverTemplate;

    [Export]
    public AudioStreamPlayer SfxSuccess;

    [Export]
    public AudioStreamPlayer SfxFail;

    private List<TargetClone> groups = new();

    private class TargetClone
    {
        public Node3D Target { get; set; }
        public Node3D Mover { get; set; }
        public Vector3 Center { get; set; }
        public Vector3 Direction { get; set; }
        public float Length { get; set; }
        public float Window { get; set; } = 0.25f;
        public float Speed { get; set; } = 1.5f;
        public Coroutine Coroutine { get; set; }
        public Vector3 TargetPosition => Center + Direction * Length;
        public Vector3 CurrentPosition => Center + Direction * Current;

        private float Current { get; set; }
        private bool IsWithinWindow => Current > MinWindow && Current < MaxWindow;
        private float MinWindow => Length - Window * 0.5f;
        private float MaxWindow => Length + Window * 0.5f;

        public Action OnSuccess;
        public Action OnFail;

        public void Clear()
        {
            Target.QueueFree();
            Mover.QueueFree();
            Coroutine.Stop(Coroutine);
        }

        public Coroutine Start()
        {
            bool success = false;
            Target.GlobalPosition = TargetPosition;
            Mover.GlobalPosition = Center;

            return Coroutine.Start(Cr);
            IEnumerator Cr()
            {
                var end = Length + Window * 0.5f + 1f;
                Current = 0f;
                while (Current < end)
                {
                    if (PlayerInput.Interact.Pressed && IsWithinWindow)
                    {
                        Mover.Hide();
                        success = true;
                        break;
                    }

                    Current += Speed * GameTime.DeltaTime;
                    Mover.GlobalPosition = CurrentPosition;

                    yield return null;
                }

                if (success)
                {
                    OnSuccess?.Invoke();
                }
                else
                {
                    OnFail?.Invoke();
                }
            }
        }
    }

    public override void _Ready()
    {
        base._Ready();
        TargetTemplate.Hide();
        MoverTemplate.Hide();
    }

    public override void Clear()
    {
        base.Clear();
        groups.ForEach(x => x.Clear());
        groups.Clear();
    }

    protected override IEnumerator Run()
    {
        yield return base.Run();

        FocusEvent.Target.Hide();

        var count = 2;
        var deg_offset = rng.RandfRange(0f, 360f);
        var length = 1.5f;
        for (int i = 0; i < count; i++)
        {
            var group = CreateGroup();
            group.Center = FocusEvent.GlobalPosition;
            group.Direction = GetDirection(i, count, deg_offset);
            group.Length = length;
            group.Coroutine = group.Start();
            group.OnSuccess = OnGroupSuccess;
            group.OnFail = OnGroupFail;

            length += rng.RandfRange(0.25f, 0.6f);
        }

        foreach (var group in groups)
        {
            yield return group.Coroutine;
        }

        FocusEvent.Target.GlobalPosition = groups.Random().CurrentPosition;
        FocusEvent.Target.Show();
    }

    private void OnGroupSuccess()
    {
        SfxSuccess.Play();
        FocusEvent.Cursor.AdjustFocusValue(0.05f);
    }

    private void OnGroupFail()
    {
        SfxFail.Play();
        FocusEvent.Cursor.AdjustFocusValue(-0.1f);
    }

    private TargetClone CreateGroup()
    {
        var group = new TargetClone
        {
            Target = CreateTarget(),
            Mover = CreateMover(),
        };

        groups.Add(group);
        return group;
    }

    private Node3D CreateTarget()
    {
        var target = TargetTemplate.Duplicate() as Node3D;
        target.SetParent(TargetTemplate.GetParent());
        target.Show();
        return target;
    }

    private Node3D CreateMover()
    {
        var mover = FocusEvent.Target.Info.Scene.Instantiate<FocusCharacter>();
        mover.SetParent(this);
        mover.Show();
        return mover;
    }

    private Vector3 GetDirection(int i, int max, float offset)
    {
        var center = FocusEvent.GlobalPosition;
        var deg = offset + 360 / max * i;
        var dir = Vector3.Forward.Rotated(Vector3.Up, Mathf.DegToRad(deg));
        return dir;
    }
}
