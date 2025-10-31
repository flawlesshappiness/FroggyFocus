using Godot;

public partial class ThirdPersonCamera : Node3D
{
    [Export]
    public Player Player;

    [Export]
    public Camera3D Camera;

    [Export]
    public SpringArm3D SpringArm;

    public static MultiLock InputLock = new();

    private float MouseSensitivity => 0.004f * Data.Options.CameraSensitivity;
    private float ControllerSensitivity => 0.02f * Data.Options.CameraSensitivity;

    private bool initialized;
    private float tilt_min = Mathf.DegToRad(-60);
    private float tilt_max = Mathf.DegToRad(-5);
    private Vector2 zoom_range = new Vector2(1f, 8f);

    private Vector3 PivotRotation
    {
        get => Rotation;
        set => Rotation = value;
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
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        Process_Position();
    }

    private void Initialize()
    {
        if (initialized) return;
        initialized = true;

        this.SetParent(Scene.Current);
    }

    private void Input_Rotation(InputEvent e)
    {
        if (e is InputEventMouseMotion mouse_motion)
        {
            var relative = mouse_motion.Relative;
            SetRotation(relative * MouseSensitivity);
        }
    }

    private void Process_Gamepad()
    {
        var input = PlayerInput.GetLookInput();
        if (input.Length() < 0.1f) return;

        SetRotation(input * ControllerSensitivity);
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
        SetZoom(SpringArm.SpringLength + value);
    }

    private void SetZoom(float value)
    {
        SpringArm.SpringLength = Mathf.Clamp(value, zoom_range.X, zoom_range.Y);
    }

    private void SetRotation(Vector2 input)
    {
        var x = Mathf.Clamp(PivotRotation.X - input.Y, tilt_min, tilt_max);
        var y = PivotRotation.Y - input.X;
        PivotRotation = new Vector3(x, y, PivotRotation.Z);
    }

    private void Process_Position()
    {
        var target = GetTargetPosition();
        GlobalPosition = GlobalPosition.Lerp(target, 10f * GameTime.DeltaTime);
    }

    public void SnapToPosition()
    {
        GlobalPosition = GetTargetPosition();
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
            var y = 0.7f + Player.Velocity.Y * 0.2f;
            y = Mathf.Max(y, 0.7f);
            return new Vector3(0, y, 0);
        }
        else
        {
            return new Vector3(0, 0.7f, 0);
        }
    }
}
