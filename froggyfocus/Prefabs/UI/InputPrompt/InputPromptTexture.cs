using Godot;
using System.Linq;

public partial class InputPromptTexture : TextureRect
{
    [Export]
    public string StartingAction;

    private string current_action;
    private bool is_gamepad;

    public override void _Ready()
    {
        base._Ready();
        PlayerInputController.Instance.OnDeviceChanged += DeviceChanged;
        VisibilityChanged += OnVisibilityChanged;

        if (!string.IsNullOrEmpty(StartingAction))
        {
            UpdateIcon(StartingAction);
        }
    }

    public bool UpdateIcon(InputEvent input)
    {
        if (input is InputEventKey key)
        {
            Show();
            return UpdateIcon(key);
        }
        else if (input is InputEventMouseButton mouse)
        {
            Show();
            return UpdateIcon(mouse);
        }
        else if (input is InputEventJoypadButton joy_button)
        {
            Show();
            return UpdateIcon(joy_button);
        }
        else
        {
            Debug.LogError($"InputPromptTexture.UpdateIcon: Failed to get InputEvent type");
            Hide();
            return false;
        }
    }

    public bool UpdateIcon(InputEventKey input)
    {
        Texture = InputIconCollection.Instance.GetIcon(input.PhysicalKeycode);
        return Texture != null;
    }

    public bool UpdateIcon(InputEventMouseButton input)
    {
        Texture = InputIconCollection.Instance.GetIcon(input.ButtonIndex);
        return Texture != null;
    }

    public bool UpdateIcon(InputEventJoypadButton input)
    {
        Texture = InputIconCollection.Instance.GetIcon(input.ButtonIndex);
        return Texture != null;
    }

    public bool UpdateIcon(string action)
    {
        current_action = action;
        var input_events = InputMap.ActionGetEvents(action);

        if (string.IsNullOrEmpty(action))
        {
            return false;
        }
        else if (input_events.Count > 0)
        {
            var kbm = input_events.FirstOrDefault(x => x is InputEventKey || x is InputEventMouseButton);
            var gamepad = input_events.FirstOrDefault(x => x is InputEventJoypadButton);
            var e = is_gamepad ? gamepad : kbm;
            return UpdateIcon(e);
        }
        else
        {
            Debug.LogError($"InputPromptTexture.UpdateIcon: Failed to get default bindings for {action}");
            return false;
        }
    }

    private void DeviceChanged(bool is_gamepad)
    {
        this.is_gamepad = is_gamepad;
        UpdateIcon(current_action);
    }

    private void OnVisibilityChanged()
    {
        UpdateIcon(current_action);
    }
}
