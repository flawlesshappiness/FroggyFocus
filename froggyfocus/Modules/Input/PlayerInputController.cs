using Godot;
using System;

public partial class PlayerInputController : SingletonController
{
    public override string Directory => $"{Paths.Modules}/Input";
    public static PlayerInputController Instance => Singleton.Get<PlayerInputController>();
    public bool IsGamepad { get; private set; }

    public Action<bool> OnDeviceChanged;

    public override void _Input(InputEvent e)
    {
        base._Input(e);

        if (e is InputEventKey)
        {
            SetGamepad(false);
        }
        else if (e is InputEventMouse)
        {
            SetGamepad(false);
        }
        else if (e is InputEventJoypadButton)
        {
            SetGamepad(true);
        }
        else if (e is InputEventJoypadMotion jmotion)
        {
            if (Mathf.Abs(jmotion.AxisValue) > Data.Options.GamepadDeadZone)
            {
                SetGamepad(true);
            }
        }
    }

    private void SetGamepad(bool gamepad)
    {
        if (IsGamepad == gamepad) return;
        IsGamepad = gamepad;
        OnDeviceChanged?.Invoke(IsGamepad);
    }
}
