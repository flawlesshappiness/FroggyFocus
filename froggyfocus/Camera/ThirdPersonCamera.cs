using Godot;
using System;
using System.Collections;

public partial class ThirdPersonCamera : Node3D
{
    [Export]
    public Player Player;

    [Export]
    public Camera3D Camera;

    [Export]
    public SpringArm3D SpringArm;

    public static MultiLock InputLock = new();

    private string DebugId => nameof(ThirdPersonCamera) + GetInstanceId();
    private float MouseSensitivity => 0.004f * Data.Options.CameraSensitivity;
    private float ControllerSensitivity => 0.09f * Data.Options.CameraSensitivity;

    private bool initialized;
    private float tilt_min = Mathf.DegToRad(-60);
    private float tilt_max = Mathf.DegToRad(-5);
    private Vector2 zoom_range = new Vector2(1f, 8f);
    private float zoom_value;
    private Vector3 interpolated_position;
    private Vector3 shake_position;

    public float ZoomOffset { get; private set; }

    private Coroutine cr_shake;
    private Coroutine cr_debug_spin;

    public class ShakeSettings
    {
        public float Frequency { get; set; }
        public float Power { get; set; }
        public float Duration { get; set; }
        public float FadeInDuration { get; set; }
        public float FadeOutDuration { get; set; }
    }

    private Vector3 PivotRotation
    {
        get => Rotation;
        set => Rotation = value;
    }

    private void Initialize()
    {
        if (initialized) return;
        initialized = true;

        zoom_value = SpringArm.SpringLength;
        interpolated_position = GlobalPosition;
        this.SetParent(Scene.Current);
        RegisterDebugActions();
    }

    private void RegisterDebugActions()
    {
        var category = "CAMERA";

        Debug.RegisterAction(new DebugAction
        {
            Id = DebugId,
            Category = category,
            Text = "Start spin animation",
            Action = StartSpinAnimation
        });

        Debug.RegisterAction(new DebugAction
        {
            Id = DebugId,
            Category = category,
            Text = "Stop spin animation",
            Action = StopSpinAnimation
        });

        void StartSpinAnimation(DebugView v)
        {
            v.Close();

            Player.FocusEventLock.AddLock(DebugId);
            InputLock.AddLock(DebugId);

            cr_debug_spin = this.StartCoroutine(Cr, "spin");
            IEnumerator Cr()
            {
                PivotRotation = new Vector3(-0.4f, 3.0f, 0f);
                SetZoom(6f);
                while (true)
                {
                    AdjustRotation(Vector2.Right * 0.003f);
                    yield return null;
                }
            }
        }

        void StopSpinAnimation(DebugView v)
        {
            v.Close();
            Player.FocusEventLock.RemoveLock(DebugId);
            InputLock.RemoveLock(DebugId);
            Coroutine.Stop(cr_debug_spin);
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        Debug.RemoveActions(DebugId);
    }

    public override void _Input(InputEvent e)
    {
        base._Input(e);

        if (InputLock.IsLocked) return;

        Input_Rotation(e);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Initialize();

        if (InputLock.IsLocked) return;

        Process_Gamepad();
        Process_Zoom();
        //Process_Position(GameTime.DeltaTime);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        var fdelta = Convert.ToSingle(delta);
        Process_Position(fdelta);
    }

    private void Input_Rotation(InputEvent e)
    {
        if (e is InputEventMouseMotion mouse_motion)
        {
            var relative = mouse_motion.Relative;
            AdjustRotation(relative * MouseSensitivity);
        }
    }

    private void Process_Gamepad()
    {
        var input = PlayerInput.GetLookInput();
        if (input.Length() < 0.1f) return;

        AdjustRotation(input * ControllerSensitivity);
    }

    private void Process_Zoom()
    {
        var zoom_controller = 0.05f;
        var zoom_mouse = zoom_controller * 8f;

        if (PlayerInput.ZoomIn_Controller.Held)
        {
            AdjustZoom(-zoom_controller);
        }
        else if (PlayerInput.ZoomOut_Controller.Held)
        {
            AdjustZoom(zoom_controller);
        }

        if (PlayerInput.ZoomIn.Pressed)
        {
            AdjustZoom(-zoom_mouse);
        }
        else if (PlayerInput.ZoomOut.Pressed)
        {
            AdjustZoom(zoom_mouse);
        }
    }

    private void AdjustZoom(float value)
    {
        SetZoom(zoom_value + value);
    }

    private void SetZoom(float value)
    {
        zoom_value = value;
        SpringArm.SpringLength = Mathf.Clamp(value, zoom_range.X, zoom_range.Y) + ZoomOffset;
    }

    public void SetZoomOffset(float value)
    {
        ZoomOffset = value;
        SetZoom(zoom_value);
    }

    private void AdjustRotation(Vector2 input)
    {
        var x = PivotRotation.X - input.Y;
        var y = PivotRotation.Y - input.X;
        SetRotation(new Vector2(x, y));
    }

    public void SetRotation(Vector2 rotation)
    {
        var x = Mathf.Clamp(rotation.X, tilt_min, tilt_max);
        PivotRotation = new Vector3(x, rotation.Y, PivotRotation.Z);
    }

    public void SetRotation(Vector3 rotation) => SetRotation(new Vector2(rotation.X, rotation.Y));

    private void Process_Position(float delta)
    {
        var speed = 4f * delta;
        var target = GetTargetPosition();
        interpolated_position = interpolated_position.Lerp(target, speed);
        GlobalPosition = interpolated_position + shake_position;
    }

    public void SnapToPosition()
    {
        interpolated_position = GetTargetPosition();
        GlobalPosition = interpolated_position;
    }

    private Vector3 GetTargetPosition()
    {
        var offset = GetOffset();
        var target = Player.GlobalPosition + offset;
        return target;
    }

    private Vector3 GetOffset()
    {
        if (Player.IsJumping)
        {
            var y = 0.7f + Player.Velocity.Y * 0.15f;
            y = Mathf.Max(y, 0.7f);
            return new Vector3(0, y, 0);
        }
        else
        {
            return new Vector3(0, 0.7f, 0);
        }
    }

    public void StartShake(ShakeSettings settings)
    {
        var rng = new RandomNumberGenerator();
        var freq_next = GameTime.Time;
        var power = 0f;
        cr_shake = this.StartCoroutine(Cr, "shake");

        IEnumerator Cr()
        {
            var next = GameTime.Time;

            yield return LerpEnumerator.Lerp01(settings.FadeInDuration, f =>
            {
                power = Mathf.Lerp(0, settings.Power, f);
                UpdateShake();
            });

            power = settings.Power;
            yield return LerpEnumerator.Lerp01(settings.Duration, f =>
            {
                UpdateShake();
            });

            yield return LerpEnumerator.Lerp01(settings.FadeOutDuration, f =>
            {
                power = Mathf.Lerp(settings.Power, 0, f);
                UpdateShake();
            });

            StopShake();
        }

        void UpdateShake()
        {
            if (GameTime.Time < freq_next) return;

            var x = rng.Randf();
            var y = rng.Randf();
            var z = rng.Randf();
            freq_next = GameTime.Time + settings.Frequency;
            shake_position = new Vector3(x, y, z) * power;
        }
    }

    public void StopShake()
    {
        Coroutine.Stop(cr_shake);
        cr_shake = null;
    }
}
