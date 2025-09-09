using Godot;

public partial class ThirdPersonCamera : Node3D
{
    [Export]
    public Camera3D Camera;

    [Export]
    public SpringArm3D SpringArm;

    public static MultiLock InputLock = new();

    private float MouseSensitivity => 0.004f * Data.Options.CameraSensitivity;
    private float ControllerSensitivity => 0.05f * Data.Options.CameraSensitivity;

    private float tilt_min = Mathf.DegToRad(-60);
    private float tilt_max = Mathf.DegToRad(-5);
    private Vector2 zoom_range = new Vector2(1f, 8f);

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (InputLock.IsLocked) return;

        if (@event is InputEventMouseMotion mouse_motion)
        {
            var relative = mouse_motion.Relative;
            var x = Mathf.Clamp(Rotation.X - relative.Y * MouseSensitivity, tilt_min, tilt_max);
            var y = Rotation.Y - relative.X * MouseSensitivity;
            Rotation = new Vector3(x, y, Rotation.Z);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (InputLock.IsLocked) return;

        Process_Controller();
        Process_Zoom();
    }

    private void Process_Controller()
    {
        var input = PlayerInput.GetLookInput();
        if (input.Length() < 0.1f) return;

        var x = Mathf.Clamp(Rotation.X - input.Y * ControllerSensitivity, tilt_min, tilt_max);
        var y = Rotation.Y - input.X * ControllerSensitivity;
        Rotation = new Vector3(x, y, Rotation.Z);
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
}
