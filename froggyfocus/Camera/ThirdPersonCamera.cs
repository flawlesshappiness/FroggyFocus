using Godot;

public partial class ThirdPersonCamera : Node3D
{
    [Export]
    public Camera3D Camera;

    [Export]
    public SpringArm3D SpringArm;

    private float MouseSensitivity => 0.005f;
    private float ControllerSensitivity => 0.05f;

    private float tilt_limit = Mathf.DegToRad(50f);
    private Vector2 zoom_range = new Vector2(1f, 8f);

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion mouse_motion)
        {
            var relative = mouse_motion.Relative;
            var x = Mathf.Clamp(Rotation.X - relative.Y * MouseSensitivity, -tilt_limit, tilt_limit);
            var y = Rotation.Y - relative.X * MouseSensitivity;
            Rotation = new Vector3(x, y, Rotation.Z);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Process_Controller();
        Process_Zoom();
    }

    private void Process_Controller()
    {
        var input = PlayerInput.GetLookInput();
        if (input.Length() < 0.1f) return;

        var x = Mathf.Clamp(Rotation.X - input.Y * ControllerSensitivity, -tilt_limit, tilt_limit);
        var y = Rotation.Y - input.X * ControllerSensitivity;
        Rotation = new Vector3(x, y, Rotation.Z);
    }

    private void Process_Zoom()
    {
        var zoom_controller = 0.05f;
        var zoom_mouse = zoom_controller * 4f;

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
