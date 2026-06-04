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
        GamepadDisplayChanged();
        OptionsContainer.OnGamepadDisplayChanged += GamepadDisplayChanged;
    }

    private void GamepadDisplayChanged()
    {
        Texture = GetGamepadTexture(Data.Options.GamepadDisplayIndex);
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
