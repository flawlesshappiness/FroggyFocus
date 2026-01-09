using Godot;

public partial class GamepadIcon : TextureRect
{
    [Export]
    public Texture2D XboxTexture;

    [Export]
    public Texture2D PlaystationTexture;

    public override void _Ready()
    {
        base._Ready();
        GamepadDisplayChanged(Data.Options.GamepadDisplayIndex);
        OptionsContainer.OnGamepadDisplayChanged += GamepadDisplayChanged;
    }

    private void GamepadDisplayChanged(int i)
    {
        Texture = GetGamepadTexture(i);
    }

    private Texture2D GetGamepadTexture(int i)
    {
        var type = (GamepadDisplayType)i;
        return type switch
        {
            GamepadDisplayType.XBox => XboxTexture,
            GamepadDisplayType.Playstation => PlaystationTexture,
            _ => XboxTexture
        };
    }
}
